using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

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
