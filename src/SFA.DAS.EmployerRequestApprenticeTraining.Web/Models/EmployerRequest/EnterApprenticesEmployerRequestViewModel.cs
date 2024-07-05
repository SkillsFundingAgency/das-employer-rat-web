using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterApprenticesEmployerRequestViewModel : SubmitEmployerRequestViewModel
    {
        public string NumberOfApprentices { get; set; }

        public string BackRoute
        {
            get
            {
                if (BackToCheckAnswers)
                    return EmployerRequestController.CheckYourAnswersRouteGet;

                return EmployerRequestController.OverviewEmployerRequestRouteGet;
            }
        }
    }
}
