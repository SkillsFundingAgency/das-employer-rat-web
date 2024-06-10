using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations
{
    public interface ILocationService
    {
        Task<Domain.Types.Locations> GetLocations(string searchTerm);

        Task<bool> CheckLocationExists(string searchTerm);
    }
}