using MediatR;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest
{
    public class GetExistingEmployerRequestQuery : IRequest<bool>
    {
        public long AccountId { get; set; }
        public string? StandardReference { get; set; }
    }
}
