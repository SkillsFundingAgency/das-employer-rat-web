using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public interface ILocationsViewModel
    {
        public string SameLocation { get; set; }
        public string SingleLocation { get; set; }
        
        public List<Region> Regions { get; set; }
        public List<string> GetMultipleLocations()
        {
            return Regions.Select(r => r.SubregionName).ToList();
        }
    }
}
