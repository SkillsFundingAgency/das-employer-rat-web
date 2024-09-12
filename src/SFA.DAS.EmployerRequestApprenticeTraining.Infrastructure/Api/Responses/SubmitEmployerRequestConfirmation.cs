namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    public class SubmitEmployerRequestConfirmation : EmployerRequestConfirmation
    {
        public string RequestedByEmail { get; set; }
        public int ExpiryAfterMonths { get; set; }
    }
}
