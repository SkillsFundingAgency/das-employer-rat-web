using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CreateEmployerRequestParameters : OverviewEmployerRequestParameters
    {
        [FromQuery]
        public bool BackToCheckAnswers { get; set; }
    }
}
