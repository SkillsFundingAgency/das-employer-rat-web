using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage
{
    public interface ISessionStorageService
    {
        EmployerRequest EmployerRequest { get; set; }
    }
}
