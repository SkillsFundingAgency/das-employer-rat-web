﻿using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CreateEmployerRequestViewModel
    {
        public string EncodedAccountId { get; set; }
        public RequestType RequestType { get; set; }
        public string StandardId { get; set; }
        public string Location { get; set; }

        public bool BackToCheckAnswers { get; set; }
    }
}
