using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models
{
    public class CreateEmployerRequestPostRequest
    {
        public string EncodedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
