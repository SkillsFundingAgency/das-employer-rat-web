using MediatR;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CancelEmployerRequest
{
    public class CancelEmployerRequestCommand : IRequest
    {
        public Guid EmployerRequestId { get; set; }
        public Guid CancelledBy { get; set; }
        public string? DashboardUrl { get; set; }
    }
}
