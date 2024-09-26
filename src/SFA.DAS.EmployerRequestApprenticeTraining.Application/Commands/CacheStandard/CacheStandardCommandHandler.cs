using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CacheStandard
{
    public class CacheStandardCommandHandler : IRequestHandler<CacheStandardCommand, Standard>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public CacheStandardCommandHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<Standard> Handle(CacheStandardCommand command, CancellationToken cancellationToken)
        {
            return await _outerApi.CacheStandard(command.StandardLarsCode);
        }
    }
}
