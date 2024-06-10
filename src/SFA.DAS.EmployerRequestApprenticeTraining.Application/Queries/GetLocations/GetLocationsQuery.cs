using MediatR;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQuery : IRequest<GetLocationsQueryResponse>
    {
        public GetLocationsQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; }
    }
}