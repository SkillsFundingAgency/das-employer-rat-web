using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitEmployerRequestParameters : Parameters
    {
        [FromQuery]
        public RequestType RequestType { get; set; }

        [FromQuery]
        public string StandardId { get; set; }

        [FromQuery]
        public string Location { get; set; }

        [FromQuery]
        public bool BackToCheckAnswers { get; set; }
    }
}
