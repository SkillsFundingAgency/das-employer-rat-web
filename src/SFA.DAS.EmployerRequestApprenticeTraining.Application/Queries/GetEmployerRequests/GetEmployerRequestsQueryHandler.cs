using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests
{
    public class GetEmployerRequestsQueryHandler : IRequestHandler<GetEmployerRequestsQuery, List<EmployerRequest>>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public GetEmployerRequestsQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<List<EmployerRequest>> Handle(GetEmployerRequestsQuery request, CancellationToken cancellationToken)
        {
            var employerRequest = await _outerApi.GetEmployerRequests(request.AccountId);

            return employerRequest;
        }
    }
}
