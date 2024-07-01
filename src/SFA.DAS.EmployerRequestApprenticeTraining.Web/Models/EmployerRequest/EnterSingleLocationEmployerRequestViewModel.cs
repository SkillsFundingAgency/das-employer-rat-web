using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterSingleLocationEmployerRequestViewModel : SubmitEmployerRequestViewModel
    {
        public string SingleLocation { get; set; }
        public string SameLocation { get; set; }

        public string BackRoute
        {
            get
            {
                if (BackToCheckAnswers)
                    return EmployerRequestController.CheckYourAnswersRouteGet;

                if (SameLocation != null)
                    return EmployerRequestController.EnterSameLocationRouteGet;

                return EmployerRequestController.EnterApprenticesRouteGet;
            }
        }
    }
}
