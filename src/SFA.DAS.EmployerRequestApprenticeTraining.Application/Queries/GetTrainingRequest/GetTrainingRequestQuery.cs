using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest
{
    public class GetTrainingRequestQuery : IRequest<TrainingRequest>
    {
        public Guid EmployerRequestId { get; set; }
    }
}
