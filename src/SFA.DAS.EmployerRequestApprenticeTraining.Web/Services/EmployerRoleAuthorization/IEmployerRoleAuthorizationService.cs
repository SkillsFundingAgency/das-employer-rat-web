using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Types;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Services.EmployerRoleAuthorization
{
    public interface IEmployerRoleAuthorizationService
    {
        Task<bool> IsEmployerAuthorized(ClaimsPrincipal user, UserRole minimumAllowedRole);
    }
}