using Microsoft.AspNetCore.Mvc;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class CancelTrainingRequestParameters : Parameters
    {
        [FromRoute]
        public Guid EmployerRequestId { get; set; }
    }
}
