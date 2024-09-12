using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public interface ITrainingOptionsViewModel
    {
        bool AtApprenticesWorkplace { get; set; }
        bool BlockRelease { get; set; }
        bool DayRelease { get; set; }

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