using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest
{
    public class SubmitEmployerRequestCommand : IRequest<Guid>
    {
        public SubmitEmployerRequestCommand(string hashedAccountId, RequestType requestType) 
        {
            HashedAccountId = hashedAccountId;
            RequestType = requestType;
        }
        
        public string HashedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
