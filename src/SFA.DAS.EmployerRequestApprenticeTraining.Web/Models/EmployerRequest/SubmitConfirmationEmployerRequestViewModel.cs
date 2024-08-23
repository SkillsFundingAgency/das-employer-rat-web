namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitConfirmationEmployerRequestViewModel : CheckYourAnswersEmployerRequestViewModel
    {
        public string FindApprenticeshipTrainingCoursesUrl { get; set; }
        public string RequestedByEmail { get; set; }
        public int ExpiryAfterMonths { get; set; }
    }
}
