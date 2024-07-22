using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterSameLocationEmployerRequestViewModel : SubmitEmployerRequestViewModel
    {
        public string SameLocation { get; set; }

        public string BackRoute
        {
            get
            {
                if (BackToCheckAnswers)
                    return EmployerRequestController.CheckYourAnswersRouteGet;

                return EmployerRequestController.EnterApprenticesRouteGet;
            }
        }
    }
}
