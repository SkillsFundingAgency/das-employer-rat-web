using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EmployerRequestApprenticeTrainingWebConfiguration
    {
        public string FindApprenticeshipTrainingBaseUrl { get; set; }
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
    }
}