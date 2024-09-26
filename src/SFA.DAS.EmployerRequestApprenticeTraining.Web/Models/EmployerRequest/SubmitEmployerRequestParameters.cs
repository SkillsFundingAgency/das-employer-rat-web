using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitEmployerRequestParameters : Parameters
    {
        [FromQuery]
        public bool BackToCheckAnswers { get; set; }
    }
}
