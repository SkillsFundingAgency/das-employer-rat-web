using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetDashboard
{
    public class GetDashboardQuery : IRequest<Dashboard>
    {   
        public long AccountId { get; set; }
    }
}
