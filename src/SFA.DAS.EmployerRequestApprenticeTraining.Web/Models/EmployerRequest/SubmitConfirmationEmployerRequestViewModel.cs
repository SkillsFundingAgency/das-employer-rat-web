namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitConfirmationEmployerRequestViewModel : CheckYourAnswersEmployerRequestViewModel
    {
        public string FindApprenticeshipTrainingBaseUrl { get; set; }
        public string RequestedByEmail { get; set; }

        public string FindApprenticeTrainingCoursesUrl => $"{FindApprenticeshipTrainingBaseUrl}courses";
    }
}
