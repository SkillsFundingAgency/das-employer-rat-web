using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(CreateEmployerRequestParameters parameters);
        void StartEmployerRequest();
        EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState);        
        void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel);
        Task<Guid> CreateEmployerRequest(CreateEmployerRequestViewModel request);
        Task<ViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId);
        Task<ViewEmployerRequestViewModel> GetViewEmployerRequestViewModel(Guid employerRequestId);
    }
}