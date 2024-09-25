using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetStandardQuery : IRequest<Standard>
    {
        public GetStandardQuery(string standardReference)
        {
            StandardReference = standardReference;
        }

        public string StandardReference { get; private set; }
    }
}
