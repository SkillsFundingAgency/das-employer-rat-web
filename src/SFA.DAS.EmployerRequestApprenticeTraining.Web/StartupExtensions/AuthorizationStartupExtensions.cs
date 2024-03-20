using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Extensions;
using System;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    public static class AuthorizationStartupExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.TransactorRole, policy =>
                {
                    policy.Requirements.Add(new TransactorRoleRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                });

                options.AddPolicy(PolicyNames.ViewerRole, policy =>
                {
                    policy.Requirements.Add(new ViewerRoleRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                });

                options.AddPolicy(PolicyNames.NoneRole, policy =>
                {
                    policy.Requirements.Add(new NoneRoleRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                });
            });

            return services;
        }
    }

    //TODO this needs to be checked to see if it is still required - but can only be checked once deployed to an environment with
    // a GOV.uk service working - or if it can be gotten working locally - is that true?
    public class AccountActiveFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;

        public AccountActiveFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null)
            {
                var isAccountSuspended = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value;
                if (isAccountSuspended != null && isAccountSuspended.Equals("Suspended", StringComparison.CurrentCultureIgnoreCase))
                {
                    context.HttpContext.Response.Redirect(RedirectExtension.GetAccountSuspendedRedirectUrl(_configuration["EnvironmentName"]));
                }
            }
        }
    }
}