using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsQueryResponse>
    {
        private readonly ILocationService _locationService;

        public GetLocationsQueryHandler(ILocationService locationService)
        {
            _locationService = locationService;
        }
        public async Task<GetLocationsQueryResponse> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            var results = await _locationService.GetLocations(request.SearchTerm);

            return new GetLocationsQueryResponse
            {
                LocationItems = results.LocationItems
            };

        }
    }
}
