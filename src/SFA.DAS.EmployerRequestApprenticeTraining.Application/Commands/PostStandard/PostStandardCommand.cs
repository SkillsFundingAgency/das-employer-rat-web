using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.PostStandard
{
    public class PostStandardCommand : IRequest<Standard>
    {
        public string StandardId { get; set; }

        public PostStandardCommand(string standardId)
        {
            StandardId = standardId;
        }
    }
}
