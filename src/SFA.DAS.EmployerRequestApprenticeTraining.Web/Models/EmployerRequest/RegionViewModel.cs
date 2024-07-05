using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class RegionViewModel
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public bool IsSelected { get; set; }

        public static implicit operator RegionViewModel(Region source)
        {
            return new RegionViewModel
            {
                Id = source.Id,
                SubregionName = source.SubregionName,
                RegionName = source.RegionName,
            };
        }
    }
}
