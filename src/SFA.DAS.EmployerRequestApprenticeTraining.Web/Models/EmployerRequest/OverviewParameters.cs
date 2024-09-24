using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class OverviewParameters:Parameters
    {
        public string StandardId { get; set; }
        public string Location { get; set; }
        public RequestType  RequestType { get; set; }
    }
}
