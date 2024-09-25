using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class OverviewParameters:Parameters
    {
        [Required]
        public string StandardId { get; set; }
        public string Location { get; set; }
        [Required]
        public RequestType  RequestType { get; set; }
    }
}
