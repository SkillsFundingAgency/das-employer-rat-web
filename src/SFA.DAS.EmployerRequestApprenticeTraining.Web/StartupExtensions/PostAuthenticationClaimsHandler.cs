using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    /// <summary>
    /// Add the required Employer claims, these claims should be the same in every application which
    /// is a subsite of the employer portal, as this handler may be called in any of the employer portal
    /// subsites the first time the user uses any of them in any particular session, but it will only be
    /// called once per session so all employer portal subsites must return exactly the same set of claims.
    /// 
    /// It is proposed in a future version of the GovUK.Auth package that these claims will be added globally
    /// to ensure that they are exactly the same.
    /// 
    /// </summary>
    public class PostAuthenticationClaimsHandler : ICustomClaims
    {
        private readonly IUserAccountsService _userAccountService;

        public PostAuthenticationClaimsHandler(IUserAccountsService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext ctx)
        {
            var claims = new List<Claim>();
            
            var userId = ctx.Principal.Claims
                    .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                    .Value;
            var email = ctx.Principal.Claims
                    .First(c => c.Type.Equals(ClaimTypes.Email))
                    .Value;

            var employerUser = await _userAccountService.GetUserAccounts(userId, email);
            if (employerUser == null)
                return claims;
            
            var employerUserAccountsAsJson = JsonConvert.SerializeObject(employerUser.EmployerUserAccounts.ToDictionary(k => k.AccountId));
            var associatedAccountsClaim = new Claim(EmployerClaims.UserAssociatedAccountsClaimsTypeIdentifier, employerUserAccountsAsJson, JsonClaimValueTypes.Json);
            claims.Add(associatedAccountsClaim);

            if (employerUser.IsSuspended)
            {
                claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
            }

            if (!string.IsNullOrEmpty(employerUser.FirstName) && !string.IsNullOrEmpty(employerUser.LastName))
            {
                claims.Add(new Claim(EmployerClaims.UserGivenNameClaimTypeIdentifier, employerUser.FirstName));
                claims.Add(new Claim(EmployerClaims.UserFamilyNameClaimTypeIdentifier, employerUser.LastName));
                claims.Add(new Claim(EmployerClaims.UserDisplayNameClaimTypeIdentifier, $"{employerUser.FirstName} {employerUser.LastName}"));
            }

            claims.Add(new Claim(EmployerClaims.UserIdClaimTypeIdentifier, employerUser.EmployerUserId));
            claims.Add(new Claim(EmployerClaims.UserEmailClaimTypeIdentifier, email));

            employerUser.EmployerUserAccounts
                .Where(c => c.Role.Equals("owner", StringComparison.CurrentCultureIgnoreCase) || c.Role.Equals("transactor", StringComparison.CurrentCultureIgnoreCase))
                .ToList().ForEach(u => claims.Add(new Claim(EmployerClaims.UserAccountClaimTypeIdentifier, u.AccountId)));

            return claims;
        }
    }
}