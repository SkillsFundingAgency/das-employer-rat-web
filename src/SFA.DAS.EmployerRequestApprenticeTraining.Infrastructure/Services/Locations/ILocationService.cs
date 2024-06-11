using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations
{
    public interface ILocationService
    {
        Task<List<LocationSearchResponse>> GetLocations(string searchTerm);

        Task<bool> CheckLocationExists(string searchTerm);
    }
}