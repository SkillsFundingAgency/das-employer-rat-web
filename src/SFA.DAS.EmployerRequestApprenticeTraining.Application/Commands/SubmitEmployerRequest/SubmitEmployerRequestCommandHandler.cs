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
                OriginalLocation = command.OriginalLocation,
                RequestType = command.RequestType,
                AccountId = command.AccountId,
                StandardReference = command.StandardReference,
                NumberOfApprentices = command.NumberOfApprentices,
                SameLocation = command.SameLocation,
                SingleLocation = command.SingleLocation,
                MultipleLocations = command.MultipleLocations,
                AtApprenticesWorkplace = command.AtApprenticesWorkplace,
                DayRelease = command.DayRelease,
                BlockRelease = command.BlockRelease,
                RequestedBy = command.RequestedBy,
                ModifiedBy = command.ModifiedBy
            });

            return employerRequestId;
        }
    }
}
