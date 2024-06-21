using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Helpers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public class EmployerRequestOrchestrator : BaseOrchestrator, IEmployerRequestOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ISessionStorageService _sessionStorage;
        private readonly ILocationService _locationService;
        private readonly EmployerRequestOrchestratorValidators _employerRequestOrchestratorValidators;
        private readonly EmployerRequestApprenticeTrainingWebConfiguration _config;

        public EmployerRequestOrchestrator(IMediator mediator, ISessionStorageService sessionStorage, 
            ILocationService locationService, IUserService userService,
            EmployerRequestOrchestratorValidators employerRequestOrchestratorValidators,
            IOptions<EmployerRequestApprenticeTrainingWebConfiguration> options)
            : base(userService)
        {
            _mediator = mediator;
            _sessionStorage = sessionStorage;
            _locationService = locationService;
            _employerRequestOrchestratorValidators = employerRequestOrchestratorValidators;
            _config = options?.Value;
        }

        public async Task<OverviewEmployerRequestViewModel> GetOverviewEmployerRequestViewModel(SubmitEmployerRequestParameters parameters)
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

        public EnterApprenticesEmployerRequestViewModel GetEnterApprenticesEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                BackToCheckAnswers = parameters.BackToCheckAnswers,
                NumberOfApprentices = EmployerRequest.NumberOfApprentices != 0 
                    ? EmployerRequest.NumberOfApprentices.ToString() 
                    : string.Empty
            };
        }

        public async Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterApprenticesEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            UpdateEmployerRequest((employerRequest) => { employerRequest.NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices); });
        }

        public EnterSingleLocationEmployerRequestViewModel GetEnterSingleLocationEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                BackToCheckAnswers = parameters.BackToCheckAnswers,
                // this is a special case where the attempted value will not automatically populate the
                // input element as the input element is being replaced with an autocomplete using javascript
                SingleLocation = modelState.GetAttemptedValueWhenInvalid(nameof(EmployerRequest.SingleLocation), EmployerRequest.SingleLocation)
            };
        }

        public async Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterSingleLocationEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            UpdateEmployerRequest((employerRequest) =>
            {
                employerRequest.SingleLocation = viewModel.SingleLocation;
            });
        }

        public EnterTrainingOptionsEmployerRequestViewModel GetEnterTrainingOptionsEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterTrainingOptionsEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                BackToCheckAnswers = parameters.BackToCheckAnswers,
                AtApprenticesWorkplace = EmployerRequest.AtApprenticesWorkplace,
                DayRelease = EmployerRequest.DayRelease,
                BlockRelease = EmployerRequest.BlockRelease
            };
        }

        public async Task<bool> ValidateEnterTrainingOptionsEmployerRequestViewModel(EnterTrainingOptionsEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterTrainingOptionsEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateTrainingOptionsForEmployerRequest(EnterTrainingOptionsEmployerRequestViewModel viewModel)
        {
            UpdateEmployerRequest((employerRequest) =>
            {
                employerRequest.AtApprenticesWorkplace = viewModel.AtApprenticesWorkplace;
                employerRequest.DayRelease = viewModel.DayRelease;
                employerRequest.BlockRelease = viewModel.BlockRelease;
            });
        }

        public async Task<CheckYourAnswersEmployerRequestViewModel> GetCheckYourAnswersEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            var standard = await _mediator.Send(new GetStandardQuery(parameters.StandardId));
            if (standard == null)
            {
                throw new ArgumentException($"The standard {parameters.StandardId} was not found");
            }

            var employerRequest = EmployerRequest;

            return new CheckYourAnswersEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                StandardTitle = standard.Title,
                StandardLevel = standard.Level,
                NumberOfApprentices = employerRequest.NumberOfApprentices > 0 ? employerRequest.NumberOfApprentices.ToString() : string.Empty,
                SingleLocation = employerRequest.SingleLocation,
                AtApprenticesWorkplace = employerRequest.AtApprenticesWorkplace,
                DayRelease = employerRequest.DayRelease,
                BlockRelease = employerRequest.BlockRelease
            };
        }

        public async Task<bool> ValidateCheckYourAnswersEmployerRequestViewModel(CheckYourAnswersEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.CheckYourAnswersEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public async Task<Guid> SubmitEmployerRequest(CheckYourAnswersEmployerRequestViewModel viewModel)
        {
            var standard = await _mediator.Send(new GetStandardQuery(viewModel.StandardId));

            var employerRequestId = await _mediator.Send(new SubmitEmployerRequestCommand
            {
                OriginalLocation = viewModel.Location,
                RequestType = viewModel.RequestType,
                AccountId = viewModel.AccountId,
                StandardReference = standard.IfateReferenceNumber,
                NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices),
                SingleLocation = viewModel.SingleLocation,
                AtApprenticesWorkplace = viewModel.AtApprenticesWorkplace,
                DayRelease = viewModel.DayRelease,
                BlockRelease = viewModel.BlockRelease,
                RequestedBy = GetCurrentUserId,
                ModifiedBy = GetCurrentUserId
            });

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