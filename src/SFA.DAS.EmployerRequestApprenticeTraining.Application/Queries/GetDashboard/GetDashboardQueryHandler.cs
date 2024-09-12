using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetDashboard
{
    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Dashboard>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public GetDashboardQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<Dashboard> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var dashboard = await _outerApi.GetDashboard(request.AccountId);
            return dashboard;
        }

    }
}
