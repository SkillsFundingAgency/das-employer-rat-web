namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public interface IEnterTrainingOptionsEmployerRequestViewModel
    {
        bool AtApprenticesWorkplace { get; set; }
        bool BlockRelease { get; set; }
        bool DayRelease { get; set; }
    }
}