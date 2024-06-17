﻿namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class EnterTrainingOptionsEmployerRequestViewModel : SubmitEmployerRequestViewModel, IEnterTrainingOptionsEmployerRequestViewModel
    {
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }
    }
}