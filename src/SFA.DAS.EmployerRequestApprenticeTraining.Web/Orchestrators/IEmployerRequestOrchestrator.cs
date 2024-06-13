﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public interface IEmployerRequestOrchestrator
    {
        Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(CreateEmployerRequestParameters parameters);
        Task StartEmployerRequest(string location);
        EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel);
        EnterSingleLocationEmployerRequestViewModel GetEnterSingleLocationEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel);
        EnterTrainingOptionsEmployerRequestViewModel GetEnterTrainingOptionsEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateEnterTrainingOptionsEmployerRequestViewModel(EnterTrainingOptionsEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        void UpdateTrainingOptionsForEmployerRequest(EnterTrainingOptionsEmployerRequestViewModel viewModel);
        Task<CheckYourAnswersEmployerRequestViewModel> GetCheckYourAnswersEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState);
        Task<bool> ValidateCheckYourAnswersEmployerRequestViewModel(CheckYourAnswersEmployerRequestViewModel viewModel, ModelStateDictionary modelState);
        Task<Guid> CreateEmployerRequest(CreateEmployerRequestViewModel request);
        Task<ViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId);
        Task<ViewEmployerRequestViewModel> GetViewEmployerRequestViewModel(Guid employerRequestId);
    }
}