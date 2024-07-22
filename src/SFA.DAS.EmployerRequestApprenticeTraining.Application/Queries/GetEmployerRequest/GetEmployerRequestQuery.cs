using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQuery : IRequest<EmployerRequest?>
    {
        public Guid? EmployerRequestId { get; set; }
        public long? AccountId { get; set; }
        public string? StandardReference { get; set; }
    }
}
