using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class OverviewEmployerRequestParameters : Parameters
    {
        [FromQuery]
        public RequestType RequestType { get; set; }

        [FromQuery]
        public string StandardId { get; set; }

        [FromQuery]
        public string Location { get; set; }
    }
}
