using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage
{
    public interface ISessionStorageService
    {
        EmployerRequest EmployerRequest { get; set; }
    }
}
