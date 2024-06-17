using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests
{
    public class SubmitEmployerRequestRequest
    {
        public string HashedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
