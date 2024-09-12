using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<DashboardViewModel> GetDashboardViewModel(long accountId, string hashedAccountId);
        Task AcknowledgeProviderResponses(Guid employerRequestId);
        Task<ViewTrainingRequestViewModel> GetViewTrainingRequestViewModel(Guid employerRequestId, string hashedAccountId);
        Task CancelTrainingRequest(Guid employerRequestId, string hashedAccountId);
        Task<CancelTrainingRequestViewModel> GetCancelTrainingRequestViewModel(Guid employerRequestId, string hashedAccountId);
        Task<CancelConfirmationEmployerRequestViewModel> GetCancelConfirmationEmployerRequestViewModel(string hashedAccountId, Guid employerRequestId);
        Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(SubmitEmployerRequestParameters parameters);
        Task<bool> HasExistingEmployerRequest(long accountId, string standardId);
        Task StartEmployerRequest(string location);
        EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel);
        EnterSameLocationEmployerRequestViewModel GetEnterSameLocationEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterSameLocationEmployerRequestViewModel(EnterSameLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateSameLocationForEmployerRequest(EnterSameLocationEmployerRequestViewModel viewModel);
        EnterSingleLocationEmployerRequestViewModel GetEnterSingleLocationEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel);
        Task<EnterMultipleLocationsEmployerRequestViewModel> GetEnterMultipleLocationsEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterMultipleLocationsEmployerRequestViewModel(EnterMultipleLocationsEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        Task UpdateMultipleLocationsForEmployerRequest(EnterMultipleLocationsEmployerRequestViewModel viewModel);
        EnterTrainingOptionsEmployerRequestViewModel GetEnterTrainingOptionsEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterTrainingOptionsEmployerRequestViewModel(EnterTrainingOptionsEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateTrainingOptionsForEmployerRequest(EnterTrainingOptionsEmployerRequestViewModel viewModel);
        Task<CheckYourAnswersEmployerRequestViewModel> GetCheckYourAnswersEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateCheckYourAnswersEmployerRequestViewModel(CheckYourAnswersEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        Task<Guid> SubmitEmployerRequest(CheckYourAnswersEmployerRequestViewModel viewModel);
        Task<SubmitConfirmationEmployerRequestViewModel> GetSubmitConfirmationEmployerRequestViewModel(string hashedAccountId, Guid employerRequestId);
    }
}