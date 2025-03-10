﻿using System;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests
{
    public class SubmitEmployerRequestRequest
    {
        public string OriginalLocation { get; set; }
        public RequestType RequestType { get; set; }
        public string StandardReference { get; set; }
        public int NumberOfApprentices { get; set; }
        public string SameLocation { get; set; }
        public string SingleLocation { get; set; }
        public string[] MultipleLocations { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }
        public Guid RequestedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public string DashboardUrl { get; set; }
    }
}
