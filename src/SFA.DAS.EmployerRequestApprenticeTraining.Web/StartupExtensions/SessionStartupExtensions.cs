using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class SessionStartupExtensions
    {
        public static IServiceCollection AddSession(this IServiceCollection services, EmployerRequestApprenticeTrainingWebConfiguration configWeb)
        {
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromMinutes(20);
                opt.Cookie = new CookieBuilder()
                {
                    Name = ".EmployerRequestApprenticeTraining.Session",
                    HttpOnly = true
                };
            });

            return services;
        }
    }
}
