using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration
{
    public class EmployerRequestApprenticeTrainingOuterApiConfiguration : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string SubscriptionKey { get; set; }

        public string ApiVersion { get; set; }
    }
}
