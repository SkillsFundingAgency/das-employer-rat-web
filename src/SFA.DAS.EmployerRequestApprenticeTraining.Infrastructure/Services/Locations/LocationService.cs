using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ILogger<LocationService> _logger;

        public LocationService(IEmployerRequestApprenticeTrainingOuterApi outerApi, ILogger<LocationService> logger)
        {
            _outerApi = outerApi;
            _logger = logger;
        }

        public async Task<List<LocationSearchResponse>> GetLocations(string searchTerm, bool exactMatch)
        {
            try
            {
                var result = await _outerApi.GetLocations(searchTerm, exactMatch);
                return result;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Unable to get locations for searchTerm:{searchTerm}");
                return new List<LocationSearchResponse>();
            }
        }

        public async Task<bool> CheckLocationExists(string searchTerm)
        {
            if(string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 3) 
                return false;

            var locations = await GetLocations(searchTerm, true);
            return locations.Exists(p => p.Name == searchTerm);
        }
    }
}