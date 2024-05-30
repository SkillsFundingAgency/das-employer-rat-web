using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Services.EmployerRoleAuthorization
{
    public class EmployerRoleAuthorizationService : IEmployerRoleAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccountsService _userAccountsService;
        private readonly ILogger<EmployerRoleAuthorizationService> _logger;

        public EmployerRoleAuthorizationService(IHttpContextAccessor httpContextAccessor, IUserAccountsService userAccountsService, ILogger<EmployerRoleAuthorizationService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userAccountsService = userAccountsService;
            _logger = logger;
        }

        public async Task<bool> IsEmployerAuthorized(ClaimsPrincipal user, UserRole minimumAllowedRole)
        {
            if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.HashedAccountId))
            {
                return false;
            }

            var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.HashedAccountId].ToString().ToUpper();
            var associatedAccountsClaim = user.FindFirst(c => c.Type.Equals(EmployerClaims.UserAssociatedAccountsClaimsTypeIdentifier));

            if (associatedAccountsClaim?.Value == null)
                return false;

            Dictionary<string, EmployerUserAccount> employerAccounts;

            try
            {
                employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccount>>(associatedAccountsClaim.Value);
            }
            catch (JsonSerializationException e)
            {
                _logger.LogError(e, "Could not deserialize employer account claim for user");
                return false;
            }

            EmployerUserAccount employerIdentifier = null;

            if (employerAccounts != null)
            {
                employerIdentifier = employerAccounts.ContainsKey(accountIdFromUrl)
                    ? employerAccounts[accountIdFromUrl] : null;
            }

            if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
            {
                var userIdClaim = user.Claims
                    .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier));

                var emailClaim = user.Claims
                    .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

                if (userIdClaim == null || emailClaim == null)
                    return false;

                var email = emailClaim.Value;
                var userId = userIdClaim.Value;

                var employerUser = await _userAccountsService.GetUserAccounts(userId, email);
                var employerUserAccounts = employerUser.EmployerUserAccounts.ToDictionary(k => k.AccountId);
                var employerUserAccountsAsJson = JsonConvert.SerializeObject(employerUserAccounts);

                userIdClaim.Subject.RemoveClaim(associatedAccountsClaim);
                associatedAccountsClaim = new Claim(EmployerClaims.UserAssociatedAccountsClaimsTypeIdentifier, employerUserAccountsAsJson, JsonClaimValueTypes.Json);
                userIdClaim.Subject.AddClaim(associatedAccountsClaim);

                if (!employerUserAccounts.ContainsKey(accountIdFromUrl))
                {
                    return false;
                }

                employerIdentifier = employerUserAccounts[accountIdFromUrl];
            }

            return CheckUserRoleForAccess(employerIdentifier, minimumAllowedRole);
        }

        private static bool CheckUserRoleForAccess(EmployerUserAccount employerIdentifier, UserRole minimumAllowedRole)
        {
            bool tryParse = Enum.TryParse<UserRole>(employerIdentifier.Role, true, out var userRole);

            if (!tryParse)
            {
                return false;
            }

            return minimumAllowedRole switch
            {
                UserRole.Owner => userRole is UserRole.Owner,
                UserRole.Transactor => userRole is UserRole.Owner || userRole is UserRole.Transactor,
                UserRole.Viewer => userRole is UserRole.Owner || userRole is UserRole.Transactor || userRole is UserRole.Viewer,
                UserRole.None => userRole is UserRole.Owner || userRole is UserRole.Transactor || userRole is UserRole.Viewer || userRole is UserRole.None,
                _ => false
            };
        }
    }
}