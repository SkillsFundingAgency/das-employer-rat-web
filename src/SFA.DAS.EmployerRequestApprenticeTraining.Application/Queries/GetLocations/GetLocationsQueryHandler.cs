using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, List<LocationSearchResponse>>
    {
        private readonly ILocationService _locationService;
        private readonly IValidator<GetLocationsQuery> _validator;

        public GetLocationsQueryHandler(ILocationService locationService, IValidator<GetLocationsQuery> validator)
        {
            _locationService = locationService;
            _validator = validator;
        }

        public async Task<List<LocationSearchResponse>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);
            var results = await _locationService.GetLocations(request.SearchTerm, false);
            return results;
        }
    }
}
