using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations
{
    public interface ILocationService
    {
        Task<List<LocationSearchResult>> GetLocations(string searchTerm, bool exactMatch);

        Task<bool> CheckLocationExists(string searchTerm);
    }
}