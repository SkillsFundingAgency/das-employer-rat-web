using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(OverviewEmployerRequestParameters parameters);
        Task<ViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId);
        Task<ViewEmployerRequestViewModel> GetViewEmployerRequestViewModel(Guid employerRequestId);
        void StartEmployerRequest();
        Task<Guid> CreateEmployerRequest(CreateEmployerRequestViewModel request);
    }
}