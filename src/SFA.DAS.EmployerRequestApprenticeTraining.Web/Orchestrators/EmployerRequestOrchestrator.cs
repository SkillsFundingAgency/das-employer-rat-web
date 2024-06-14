using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
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
        private readonly IValidator<EnterApprenticesEmployerRequestViewModel> _enterApprenticesEmployerRequestViewModelValidator;
        private readonly EmployerRequestApprenticeTrainingWebConfiguration _config;

        public EmployerRequestOrchestrator(IMediator mediator, ISessionStorageService sessionStorage, 
            IValidator<EnterApprenticesEmployerRequestViewModel> enterApprenticesEmployerRequestViewModelValidator,
            IOptions<EmployerRequestApprenticeTrainingWebConfiguration> options)
        {
            _mediator = mediator;
            _sessionStorage = sessionStorage;
            _enterApprenticesEmployerRequestViewModelValidator = enterApprenticesEmployerRequestViewModelValidator;
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

        public void StartEmployerRequest()
        {
            _sessionStorage.EmployerRequest = new EmployerRequest();
        }

        public EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(CreateEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            var employerRequest = _sessionStorage.EmployerRequest ?? new EmployerRequest();

            return new EnterApprenticesEmployerRequestViewModel
            { 
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                NumberOfApprentices = employerRequest.NumberOfApprentices.ToString()
            };
        }

        public async Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            await _enterApprenticesEmployerRequestViewModelValidator.ValidateAndAddModelErrorsAsync(viewModel, modelState);
            return modelState.IsValid;
        }

        public void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            var employerRequest = _sessionStorage.EmployerRequest ?? new EmployerRequest();
            employerRequest.NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices);
            _sessionStorage.EmployerRequest = employerRequest;
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
    }
}