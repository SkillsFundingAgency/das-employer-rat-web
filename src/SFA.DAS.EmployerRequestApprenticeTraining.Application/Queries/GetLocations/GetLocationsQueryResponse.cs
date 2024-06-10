namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQueryResponse
    {
        public IEnumerable<Domain.Types.Locations.LocationItem>? LocationItems { get; set; }
    }
}