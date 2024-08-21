using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.AcknowledgeProviderResponses
{
    public class AcknowledgeProviderResponsesCommandHandler : IRequestHandler<AcknowledgeProviderResponsesCommand>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public AcknowledgeProviderResponsesCommandHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task Handle(AcknowledgeProviderResponsesCommand command, CancellationToken cancellationToken)
        {
            await _outerApi.AcknowledgeProviderResponses(command.EmployerRequestId, command.AcknowledgedBy);
        }
    }
}
