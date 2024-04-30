﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class CacheStartupExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IHostEnvironment environment, EmployerRequestApprenticeTrainingWebConfiguration config)
        {
            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options => { options.Configuration = config.RedisConnectionString; });
            }

            return services;
        }
    }
}