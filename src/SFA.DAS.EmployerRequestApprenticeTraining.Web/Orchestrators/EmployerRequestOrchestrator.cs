﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Helpers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public class EmployerRequestOrchestrator : IEmployerRequestOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ISessionStorageService _sessionStorage;
        private readonly ILocationService _locationService;
        private readonly IValidator<EnterApprenticesEmployerRequestViewModel> _enterApprenticesEmployerRequestViewModelValidator;
        private readonly IValidator<EnterSingleLocationEmployerRequestViewModel> _enterSingleLocationEmployerRequestViewModelValidator;
        private readonly EmployerRequestApprenticeTrainingWebConfiguration _config;

        public EmployerRequestOrchestrator(IMediator mediator, ISessionStorageService sessionStorage, ILocationService locationService,
            IValidator<EnterApprenticesEmployerRequestViewModel> enterApprenticesEmployerRequestViewModelValidator,
            IValidator<EnterSingleLocationEmployerRequestViewModel> enterSingleLocationEmployerRequestViewModelValidator,
            IOptions<EmployerRequestApprenticeTrainingWebConfiguration> options)
        {
            _mediator = mediator;
            _sessionStorage = sessionStorage;
            _locationService = locationService;
            _enterApprenticesEmployerRequestViewModelValidator = enterApprenticesEmployerRequestViewModelValidator;
            _enterSingleLocationEmployerRequestViewModelValidator = enterSingleLocationEmployerRequestViewModelValidator;
            _config = options?.Value;
        }

        public async Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(CreateEmployerRequestParameters parameters)
        {
            var standard = await _mediator.Send(new GetStandardQuery(parameters.StandardId));
            if (standard == null)
            {
                throw new ArgumentException($"The standard {parameters.StandardId} was not found");
            }

            return new OverviewEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                RequestType = parameters.RequestType,
                StandardId = parameters.StandardId,
                Location = parameters.Location,
                StandardTitle = standard.Title,
                StandardLevel = standard.Level,
                StandardLarsCode = standard.LarsCode,
                FindApprenticeshipTrainingBaseUrl = _config?.FindApprenticeshipTrainingBaseUrl
            };
        }

        public async Task StartEmployerRequest(string location)
        {
            _sessionStorage.EmployerRequest = new EmployerRequest
            {
                SingleLocation = await _locationService.CheckLocationExists(location) ? location : string.Empty
            };
        }

        public EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterApprenticesEmployerRequestViewModel
            { 
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                NumberOfApprentices = EmployerRequest.NumberOfApprentices.ToString()
            };
        }

        public async Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_enterApprenticesEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            UpdateEmployerRequest((employerRequest) => { employerRequest.NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices); });
        }

        public EnterSingleLocationEmployerRequestViewModel GetEnterSingleLocationEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                // this is a special case where the attempted value will not automatically populate the
                // input element as the input element is being replaced with an autocomplete using javascript
                SingleLocation = modelState.GetAttemptedValueWhenInvalid(nameof(EmployerRequest.SingleLocation), EmployerRequest.SingleLocation)
            };
        }

        public async Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_enterSingleLocationEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            UpdateEmployerRequest((employerRequest) => 
            { 
                employerRequest.SingleLocation = viewModel.SingleLocation;
            });
        }

        public async Task<Guid> CreateEmployerRequest(CreateEmployerRequestViewModel request)
        {
            var employerRequestId = await _mediator.Send(new CreateEmployerRequestCommand(
                request.HashedAccountId, request.RequestType
            ));

            return employerRequestId;
        }

        public async Task<ViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId)
        {
            var result = await _mediator.Send(new GetEmployerRequestsQuery { AccountId = accountId });

            return new ViewEmployerRequestsViewModel()
            {
                EmployerRequests = result
            };
        }

        public async Task<ViewEmployerRequestViewModel> GetViewEmployerRequestViewModel(Guid employerRequestId)
        {
            var result = await _mediator.Send(new GetEmployerRequestQuery { EmployerRequestId = employerRequestId });
            return new ViewEmployerRequestViewModel
            {
                EmployerRequestId = result.Id,
                AccountId = result.AccountId,
                RequestType = result.RequestType
            };
        }

        private EmployerRequest EmployerRequest
        {
            get => _sessionStorage.EmployerRequest ?? new EmployerRequest();
        }

        private void UpdateEmployerRequest(Action<EmployerRequest> action)
        {
            var employerRequest = EmployerRequest;
            action(employerRequest);
            _sessionStorage.EmployerRequest = employerRequest;
        }

        private async Task<bool> ValidateViewModel<T>(IValidator<T> validator, T viewModel, ModelStateDictionary modelState)
        {
            await validator.ValidateAndAddModelErrorsAsync(viewModel, modelState);
            return modelState.IsValid;
        }
    }
}