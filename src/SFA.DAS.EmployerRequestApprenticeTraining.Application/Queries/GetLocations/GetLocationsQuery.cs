using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQuery : IRequest<List<LocationSearchResponse>>
    {
        public GetLocationsQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; }
    }
}