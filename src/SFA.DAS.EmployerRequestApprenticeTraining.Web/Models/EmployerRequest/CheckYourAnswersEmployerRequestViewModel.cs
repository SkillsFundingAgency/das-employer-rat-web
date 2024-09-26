using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CheckYourAnswersEmployerRequestViewModel : SubmitEmployerRequestViewModel, ITrainingOptionsViewModel, ILocationsViewModel
    {
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public string NumberOfApprentices { get; set; }
        public string SameLocation { get; set; }
        public string SingleLocation { get; set; }
        public string[] MultipleLocations { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }

        public List<Region> Regions { get; set; }
    }
}
