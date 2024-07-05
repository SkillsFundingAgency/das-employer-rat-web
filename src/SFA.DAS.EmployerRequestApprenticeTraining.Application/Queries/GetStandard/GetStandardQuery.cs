using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetStandardQuery : IRequest<Standard>
    {
        public GetStandardQuery(string standardId)
        {
            StandardId = standardId;
        }

        public string StandardId { get; private set; }
    }
}
