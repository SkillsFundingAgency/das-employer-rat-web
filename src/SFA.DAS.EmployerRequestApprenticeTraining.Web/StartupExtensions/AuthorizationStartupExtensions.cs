﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.GovUK.Auth.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
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
}