using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQueryHandler : IRequestHandler<GetEmployerRequestQuery, EmployerRequest>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public GetEmployerRequestQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<EmployerRequest> Handle(GetEmployerRequestQuery request, CancellationToken cancellationToken)
        {
            var employerRequest = await _outerApi.GetEmployerRequest(request.EmployerRequestId);

            return employerRequest;
        }
    }
}
