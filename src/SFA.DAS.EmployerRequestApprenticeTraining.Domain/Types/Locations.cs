using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    public class Locations
    {
        [JsonProperty("locations")]
        public List<LocationItem> LocationItems { get; set; }

        public class LocationItem
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
