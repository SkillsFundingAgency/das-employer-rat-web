﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Shared
{
    [ExcludeFromCodeCoverage]
    public class GaData
    {
        public string DataLoaded { get; set; } = "dataLoaded";
        public string UserId { get; set; }
        public string Vpv { get; set; }
        public string Acc { get; set; }
        public string Org { get; set; }
        public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
    }
}
