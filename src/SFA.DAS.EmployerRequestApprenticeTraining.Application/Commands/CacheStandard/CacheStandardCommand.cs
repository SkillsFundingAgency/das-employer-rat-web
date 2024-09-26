using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CacheStandard
{
    public class CacheStandardCommand : IRequest<Standard>
    {
        public string StandardLarsCode { get; set; }

        public CacheStandardCommand(string standardLarsCode)
        {
            StandardLarsCode = standardLarsCode;
        }
    }
}
