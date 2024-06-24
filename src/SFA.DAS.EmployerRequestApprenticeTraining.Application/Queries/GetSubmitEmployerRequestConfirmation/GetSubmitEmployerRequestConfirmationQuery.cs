using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation
{
    public class GetSubmitEmployerRequestConfirmationQuery : IRequest<SubmitEmployerRequestConfirmationResponse>
    {
        public Guid EmployerRequestId { get; set; }
    }
}
