using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest
{
    public class CreateEmployerRequestCommand : IRequest<Guid>
    {
        public CreateEmployerRequestCommand(string hashedAccountId, RequestType requestType) 
        {
            HashedAccountId = hashedAccountId;
            RequestType = requestType;
        }
        
        public string HashedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
