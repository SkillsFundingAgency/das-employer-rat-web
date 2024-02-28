using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest
{
    public class CreateEmployerRequestCommand : IRequest<Guid>
    {
        public CreateEmployerRequestCommand(string encodedAccountId, RequestType requestType) 
        {
            EncodedAccountId = encodedAccountId;
            RequestType = requestType;
        }
        
        public string EncodedAccountId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
