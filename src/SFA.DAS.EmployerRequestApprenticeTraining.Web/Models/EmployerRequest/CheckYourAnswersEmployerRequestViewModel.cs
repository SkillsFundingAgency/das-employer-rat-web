using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CheckYourAnswersEmployerRequestViewModel : CreateEmployerRequestViewModel, IEnterTrainingOptionsEmployerRequestViewModel
    {
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public string NumberOfApprentices { get; set; }
        public string SingleLocation { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }

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
