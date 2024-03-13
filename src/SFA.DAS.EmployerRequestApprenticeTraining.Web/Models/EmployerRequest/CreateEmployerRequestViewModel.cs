using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models
{
    public class CreateEmployerRequestViewModel
    {
        public string EncodedAccountId { get; set; }
        public List<RequestType> RequestTypes { get; set; }
        public RequestType RequestType { get; set; }
    }
}