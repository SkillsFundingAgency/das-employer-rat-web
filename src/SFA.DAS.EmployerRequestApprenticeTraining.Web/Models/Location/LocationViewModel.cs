using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Location
{
    public class LocationViewModel
    {
        public string Name { get; set; }
        
        public static implicit operator LocationViewModel(LocationSearchResponse source)
        {
            return new LocationViewModel
            {
                Name = source.Name,
            };
        }
    }
}
