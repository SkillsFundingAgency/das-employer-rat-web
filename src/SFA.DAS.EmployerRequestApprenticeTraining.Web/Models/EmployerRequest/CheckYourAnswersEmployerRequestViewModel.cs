using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CheckYourAnswersEmployerRequestViewModel : SubmitEmployerRequestViewModel, IEnterTrainingOptionsEmployerRequestViewModel
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
        public List<string> GetMultipleLocations()
        {
            return Regions.Select(r => r.SubregionName).ToList();
        }

        public List<string> GetTrainingOptions()
        {
            var trainingOptions = new List<string>();
            
            if (AtApprenticesWorkplace) trainingOptions.Add(TrainingOptions.AtApprenticesWorkplace);
            if (DayRelease) trainingOptions.Add(TrainingOptions.DayRelease);
            if (BlockRelease) trainingOptions.Add(TrainingOptions.BlockRelease);
            
            return trainingOptions;
        }
    }
}
