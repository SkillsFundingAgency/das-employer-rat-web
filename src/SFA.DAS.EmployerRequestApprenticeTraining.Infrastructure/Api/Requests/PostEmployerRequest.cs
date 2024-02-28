using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests
{
    public class PostEmployerRequest
    {
        public string EncodedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
