using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class SessionStartupExtensions
    {
        public static IServiceCollection AddSessionOptions(this IServiceCollection services)
        {
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(20);
                opt.Cookie = new CookieBuilder()
                {
                    Name = ".EmployerRequestApprenticeTraining.Session",
                    HttpOnly = true,
                    SecurePolicy = CookieSecurePolicy.Always
                };
            });

            return services;
        }
    }
}