using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion
{
    public class GetClosestRegionQuery : IRequest<Region?>
    {
        public string? Location { get; set; }
    }
}
