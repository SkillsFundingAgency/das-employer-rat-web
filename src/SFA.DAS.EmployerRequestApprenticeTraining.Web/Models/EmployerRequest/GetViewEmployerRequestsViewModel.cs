using System.Collections.Generic;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models
{
    public class GetViewEmployerRequestsViewModel
    {
        public List<EmployerRequest> EmployerRequests { get; set; }
    }
}
