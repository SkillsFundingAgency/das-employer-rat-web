using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest
{
    public class SubmitEmployerRequestCommandHandler : IRequestHandler<SubmitEmployerRequestCommand, Guid>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public SubmitEmployerRequestCommandHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<Guid> Handle(SubmitEmployerRequestCommand command, CancellationToken cancellationToken)
        {
            var employerRequestId = await _outerApi.SubmitEmployerRequest(new SubmitEmployerRequestRequest
            { 
                HashedAccountId = command.HashedAccountId,
                RequestType = command.RequestType
            });

            return employerRequestId;
        }
    }
}
