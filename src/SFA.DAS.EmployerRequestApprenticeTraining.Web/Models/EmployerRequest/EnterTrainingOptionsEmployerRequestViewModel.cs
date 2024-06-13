namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterTrainingOptionsEmployerRequestViewModel : CreateEmployerRequestViewModel, IEnterTrainingOptionsEmployerRequestViewModel
    {
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }
    }
}
