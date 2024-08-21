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
            var aggregatedEmployerRequestsTask = _outerApi.GetAggregatedEmployerRequests(request.AccountId);
            var settingsTask = _outerApi.GetSettings();

            await Task.WhenAll(aggregatedEmployerRequestsTask, settingsTask);
            
            var aggregatedEmployerRequests = await aggregatedEmployerRequestsTask;
            var settings = await settingsTask;

            return new Dashboard
            {
                AggregatedEmployerRequests = aggregatedEmployerRequests,
                Settings = settings
            };
        }

    }
}
