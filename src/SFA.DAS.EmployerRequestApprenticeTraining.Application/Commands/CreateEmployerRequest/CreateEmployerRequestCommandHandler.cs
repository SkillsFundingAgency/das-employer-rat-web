using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest
{
    public class CreateEmployerRequestCommandHandler : IRequestHandler<CreateEmployerRequestCommand, Guid>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public CreateEmployerRequestCommandHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<Guid> Handle(CreateEmployerRequestCommand command, CancellationToken cancellationToken)
        {
            var employerRequestId = await _outerApi.CreateEmployerRequest(new PostEmployerRequest 
            { 
                HashedAccountId = command.HashedAccountId,
                RequestType = command.RequestType
            });

            return employerRequestId;
        }
    }
}
