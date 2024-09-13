namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    public class Settings
    {
        public int ExpiryAfterMonths { get; set; }
        public int RemovedAfterExpiryNoResponsesMonths { get; set; }
        public int RemovedAfterExpiryResponsesMonths { get; set; }
    }
}
