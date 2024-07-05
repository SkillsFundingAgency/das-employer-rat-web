using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions
{
    public class GetRegionsQuery : IRequest<List<Region>>
    { 
    }
}
