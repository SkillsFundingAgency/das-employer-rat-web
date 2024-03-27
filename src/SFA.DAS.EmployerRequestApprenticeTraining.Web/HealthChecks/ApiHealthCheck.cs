using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public ApiHealthCheck(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            var description = "Ping of Employer Request Apprentice Training outer API";

            try
            {
                await _outerApi.Ping();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description, ex);
            }

            return HealthCheckResult.Healthy(description, new Dictionary<string, object>());
        }
    }
}