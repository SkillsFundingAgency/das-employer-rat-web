using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(SubmitEmployerRequestParameters parameters);
        Task StartEmployerRequest(string location);
        EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel);
        EnterSingleLocationEmployerRequestViewModel GetEnterSingleLocationEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel);
        EnterTrainingOptionsEmployerRequestViewModel GetEnterTrainingOptionsEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterTrainingOptionsEmployerRequestViewModel(EnterTrainingOptionsEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateTrainingOptionsForEmployerRequest(EnterTrainingOptionsEmployerRequestViewModel viewModel);
        Task<CheckYourAnswersEmployerRequestViewModel> GetCheckYourAnswersEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateCheckYourAnswersEmployerRequestViewModel(CheckYourAnswersEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        Task<Guid> SubmitEmployerRequest(CheckYourAnswersEmployerRequestViewModel viewModel);
        Task<ViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId);
        Task<ViewEmployerRequestViewModel> GetViewEmployerRequestViewModel(Guid employerRequestId);
        Task<SubmitConfirmationEmployerRequestViewModel> GetSubmitConfirmationEmployerRequestViewModel(Guid employerRequestId);
    }
}