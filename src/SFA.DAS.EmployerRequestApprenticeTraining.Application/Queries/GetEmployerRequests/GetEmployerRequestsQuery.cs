using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests
{
    public class GetEmployerRequestsQuery : IRequest<List<EmployerRequest>>
    {
        public long AccountId { get; set; }
    }
}
