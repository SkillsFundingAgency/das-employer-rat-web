using static SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types.Locations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Location
{
    public class LocationViewModel
    {
        public string Name { get; set; }
        
        public static implicit operator LocationViewModel(LocationItem source)
        {
            return new LocationViewModel
            {
                Name = source.Name,
            };
        }
    }
}
