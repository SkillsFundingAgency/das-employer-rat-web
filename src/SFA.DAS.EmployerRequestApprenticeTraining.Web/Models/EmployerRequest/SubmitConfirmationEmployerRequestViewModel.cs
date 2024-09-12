namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitConfirmationEmployerRequestViewModel : ConfirmationEmployerRequestViewModel
    {
        public string RequestedByEmail { get; set; }
        public int ExpiryAfterMonths { get; set; }
    }
}
