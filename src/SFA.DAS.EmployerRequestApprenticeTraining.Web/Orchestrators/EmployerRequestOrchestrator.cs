using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Helpers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Collections.Generic;
using System.Linq;
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
                NumberOfApprentices = SessionEmployerRequest.NumberOfApprentices != 0 
                    ? SessionEmployerRequest.NumberOfApprentices.ToString() 
                    : string.Empty
            };
        }

        public async Task<bool> ValidateEnterApprenticesEmployerRequestViewModel(EnterApprenticesEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterApprenticesEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateNumberOfApprenticesForEmployerRequest(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            UpdateSessionEmployerRequest((employerRequest) => 
            {
                employerRequest.NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices);
                
                if(employerRequest.NumberOfApprentices == 1)
                {
                    employerRequest.SameLocation = null;
                    employerRequest.Regions = null;
                }

                if(employerRequest.NumberOfApprentices > 1)
                {
                    employerRequest.SingleLocation = null;
                }
            });
        }

        public EnterSameLocationEmployerRequestViewModel GetEnterSameLocationEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            return new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                BackToCheckAnswers = parameters.BackToCheckAnswers,
                SameLocation = SessionEmployerRequest.SameLocation
            };
        }

        public async Task<bool> ValidateEnterSameLocationEmployerRequestViewModel(EnterSameLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterSameLocationEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateSameLocationForEmployerRequest(EnterSameLocationEmployerRequestViewModel viewModel)
        {
            UpdateSessionEmployerRequest((employerRequest) => 
            { 
                employerRequest.SameLocation = viewModel.SameLocation; 
                if(employerRequest.SameLocation == "Yes")
                {
                    employerRequest.Regions = null;
                }

                if(employerRequest.SameLocation == "No")
                {
                    employerRequest.SingleLocation = null;
                }
            });
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
                SingleLocation = modelState.GetAttemptedValueWhenInvalid(nameof(SessionEmployerRequest.SingleLocation), SessionEmployerRequest.SingleLocation),
                SameLocation = SessionEmployerRequest.SameLocation
            };
        }

        public async Task<bool> ValidateEnterSingleLocationEmployerRequestViewModel(EnterSingleLocationEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterSingleLocationEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateSingleLocationForEmployerRequest(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            UpdateSessionEmployerRequest((employerRequest) =>
            {
                employerRequest.SingleLocation = viewModel.SingleLocation;
            });
        }

        public async Task<EnterMultipleLocationsEmployerRequestViewModel> GetEnterMultipleLocationsEmployerRequestViewModel(SubmitEmployerRequestParameters parameters, ModelStateDictionary modelState)
        {
            var regions = await _mediator.Send(new GetRegionsQuery());

            if(!string.IsNullOrEmpty(parameters.Location) && !(SessionEmployerRequest.Regions?.Any() ?? false)) 
            {
                var closestRegion = await _mediator.Send(new GetClosestRegionQuery {  Location = parameters.Location });
                if (closestRegion != null)
                {
                    UpdateSessionEmployerRequest((employerRequest) =>
                    {
                        employerRequest.Regions = new List<Region> { closestRegion };
                    });
                }
            }

            return new EnterMultipleLocationsEmployerRequestViewModel(regions.Select(r => (RegionViewModel)r).ToList())
            {
                HashedAccountId = parameters.HashedAccountId,
                StandardId = parameters.StandardId,
                RequestType = parameters.RequestType,
                Location = parameters.Location,
                BackToCheckAnswers = parameters.BackToCheckAnswers,
                SelectedSubRegions = SessionEmployerRequest.Regions?.Select(r => r.Id.ToString()).ToArray() ?? []
        };
        }

        public async Task<bool> ValidateEnterMultipleLocationsEmployerRequestViewModel(EnterMultipleLocationsEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterMultipleLocationsEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateMultipleLocationsForEmployerRequest(EnterMultipleLocationsEmployerRequestViewModel viewModel)
        {
            UpdateSessionEmployerRequest((employerRequest) =>
            {
                employerRequest.Regions = viewModel.SelectedSubRegions.Select(s => new Region { Id = int.Parse(s) }).ToList();
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
                AtApprenticesWorkplace = SessionEmployerRequest.AtApprenticesWorkplace,
                DayRelease = SessionEmployerRequest.DayRelease,
                BlockRelease = SessionEmployerRequest.BlockRelease,
                SameLocation = SessionEmployerRequest.SameLocation
            };
        }

        public async Task<bool> ValidateEnterTrainingOptionsEmployerRequestViewModel(EnterTrainingOptionsEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterTrainingOptionsEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public void UpdateTrainingOptionsForEmployerRequest(EnterTrainingOptionsEmployerRequestViewModel viewModel)
        {
            UpdateSessionEmployerRequest((employerRequest) =>
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

            var employerRequest = SessionEmployerRequest;

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

            ClearSessionEmployerRequest();

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

        private EmployerRequest SessionEmployerRequest
        {
            get => _sessionStorage.EmployerRequest ?? new EmployerRequest();
        }

        private void UpdateSessionEmployerRequest(Action<EmployerRequest> action)
        {
            var employerRequest = SessionEmployerRequest;
            action(employerRequest);
            _sessionStorage.EmployerRequest = employerRequest;
        }

        private void ClearSessionEmployerRequest()
        {
            _sessionStorage.EmployerRequest = null;
        }

        private async Task<bool> ValidateViewModel<T>(IValidator<T> validator, T viewModel, ModelStateDictionary modelState)
        {
            await validator.ValidateAndAddModelErrorsAsync(viewModel, modelState);
            return modelState.IsValid;
        }

        public async Task<SubmitConfirmationEmployerRequestViewModel> GetSubmitConfirmationEmployerRequestViewModel(Guid employerRequestId)
        {
            var result = await _mediator.Send(new GetSubmitEmployerRequestConfirmationQuery { EmployerRequestId = employerRequestId });
            if(result == null)
            {
                throw new ArgumentException($"The employer request {employerRequestId} was not found");
            }

            return new SubmitConfirmationEmployerRequestViewModel
            {
                StandardTitle = result.StandardTitle,
                StandardLevel = result.StandardLevel,
                NumberOfApprentices = result.NumberOfApprentices.ToString(),
                SingleLocation = result.SingleLocation,
                AtApprenticesWorkplace = result.AtApprenticesWorkplace,
                DayRelease = result.DayRelease,
                BlockRelease = result.BlockRelease,
                RequestedByEmail = result.RequestedByEmail,
                FindApprenticeshipTrainingBaseUrl = _config?.FindApprenticeshipTrainingBaseUrl
            };
        }
    }
}