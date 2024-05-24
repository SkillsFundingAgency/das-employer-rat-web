using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models
{
    public class ViewEmployerRequestViewModel
    {
        public long AccountId { get; set; }
        public Guid EmployerRequestId { get; set; }
        public RequestType RequestType { get; set; }
    }
}