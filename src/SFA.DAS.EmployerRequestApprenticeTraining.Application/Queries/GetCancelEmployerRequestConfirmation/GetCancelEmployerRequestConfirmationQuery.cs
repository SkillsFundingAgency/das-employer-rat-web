using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetCancelEmployerRequestConfirmation
{
    public class GetCancelEmployerRequestConfirmationQuery : IRequest<CancelEmployerRequestConfirmation>
    {
        public Guid EmployerRequestId { get; set; }
    }
}
