using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage
{
    public interface ISessionStorageService
    {
        EmployerRequest EmployerRequest { get; set; }
    }
}
