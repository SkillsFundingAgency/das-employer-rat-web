﻿using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    public class EmployerRequestConfirmation
    {
        public Guid EmployerRequestId { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public int NumberOfApprentices { get; set; }
        public string SameLocation { get; set; }
        public string SingleLocation { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }

        public List<Region> Regions { get; set; }
    }
}
