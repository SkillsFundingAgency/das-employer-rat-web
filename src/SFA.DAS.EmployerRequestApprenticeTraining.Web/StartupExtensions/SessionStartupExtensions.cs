using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
                    SecurePolicy = CookieSecurePolicy.Always,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                    IsEssential = true
                };
            });

            services.AddAntiforgery(opt =>
            {
                opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            return services;
        }
    }
}
