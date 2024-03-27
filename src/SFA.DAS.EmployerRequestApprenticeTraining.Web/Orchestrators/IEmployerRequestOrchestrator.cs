using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<GetViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long acountId);
        Task<Guid> CreateEmployerRequest(CreateEmployerRequestPostRequest postRequest);
        CreateEmployerRequestViewModel GetCreateEmployerRequestViewModel(string encodedAccountId);
        Task<GetEmployerRequestViewModel> GetEmployerRequestViewModel(Guid employerRequestId);
    }
}