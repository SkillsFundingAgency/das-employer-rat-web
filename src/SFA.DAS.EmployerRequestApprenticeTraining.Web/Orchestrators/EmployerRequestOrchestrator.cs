using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.AcknowledgeProviderResponses;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CancelEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetCancelEmployerRequestConfirmation;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetDashboard;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Helpers;
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
        private readonly UrlBuilder _urlBuilder;

        public EmployerRequestOrchestrator(IMediator mediator, ISessionStorageService sessionStorage, 
            ILocationService locationService, IUserService userService,
            EmployerRequestOrchestratorValidators employerRequestOrchestratorValidators,
            IOptions<EmployerRequestApprenticeTrainingWebConfiguration> options, UrlBuilder urlBuilder)
            : base(userService)
        {
            _mediator = mediator;
            _sessionStorage = sessionStorage;
            _locationService = locationService;
            _employerRequestOrchestratorValidators = employerRequestOrchestratorValidators;
            _config = options?.Value;
            _urlBuilder = urlBuilder;
        }

        public async Task<DashboardViewModel> GetDashboardViewModel(long accountId, string hashedAccountId)
        {
            var dashboard = await _mediator.Send(new GetDashboardQuery { AccountId = accountId });
            return new DashboardViewModel 
            { 
                Dashboard = dashboard,
                HashedAccountId = hashedAccountId,
                FindApprenticeshipTrainingCoursesUrl = $"{_config.FindApprenticeshipTrainingBaseUrl}courses",
                EmployerAccountDashboardUrl = _urlBuilder.AccountsLink("AccountsHome", hashedAccountId)
            };
        }

        public async Task AcknowledgeProviderResponses(Guid employerRequestId)
        {
            await _mediator.Send(new AcknowledgeProviderResponsesCommand 
            { 
                EmployerRequestId = employerRequestId, 
                AcknowledgedBy = GetCurrentUserId 
            });
        }

        public async Task CancelTrainingRequest(Guid employerRequestId, string hashedAccountId)
        {
            await _mediator.Send(new CancelEmployerRequestCommand
            {
                EmployerRequestId = employerRequestId,
                CancelledBy = GetCurrentUserId,
                DashboardUrl = _urlBuilder.RequestApprenticeshipTrainingLink("Dashboard", hashedAccountId)
            });
        }

        public async Task<CancelConfirmationEmployerRequestViewModel> GetCancelConfirmationEmployerRequestViewModel(string hashedAccountId, Guid employerRequestId)
        {
            var result = await _mediator.Send(new GetCancelEmployerRequestConfirmationQuery { EmployerRequestId = employerRequestId });
            if (result == null)
            {
                throw new ArgumentException($"The employer request {employerRequestId} was not found");
            }

            return new CancelConfirmationEmployerRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                StandardTitle = result.StandardTitle,
                StandardLevel = result.StandardLevel,
                NumberOfApprentices = result.NumberOfApprentices.ToString(),
                SameLocation = result.SameLocation,
                SingleLocation = result.SingleLocation,
                AtApprenticesWorkplace = result.AtApprenticesWorkplace,
                DayRelease = result.DayRelease,
                BlockRelease = result.BlockRelease,
                CancelledByEmail = result.CancelledByEmail,
                FindApprenticeshipTrainingCoursesUrl = $"{_config.FindApprenticeshipTrainingBaseUrl}courses",
                Regions = result.Regions
            };
        }

        public async Task<ViewTrainingRequestViewModel> GetViewTrainingRequestViewModel(Guid employerRequestId, string hashedAccountId)
        {
            var trainingRequest = await _mediator.Send(new GetTrainingRequestQuery { EmployerRequestId = employerRequestId, IncludeProviders = true });
            if (trainingRequest == null)
            {
                throw new ArgumentException($"The training request for {employerRequestId} was not found");
            }

            return new ViewTrainingRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                EmployerRequestId = trainingRequest.EmployerRequestId,
                StandardTitle = trainingRequest.StandardTitle,
                StandardLevel = trainingRequest.StandardLevel,
                NumberOfApprentices = trainingRequest.NumberOfApprentices,
                SameLocation = trainingRequest.SameLocation,
                SingleLocation = trainingRequest.SingleLocation,
                AtApprenticesWorkplace = trainingRequest.AtApprenticesWorkplace,
                DayRelease = trainingRequest.DayRelease,
                BlockRelease = trainingRequest.BlockRelease,
                RequestedAt = trainingRequest.RequestedAt,
                Status = trainingRequest.Status,
                ExpiredAt = trainingRequest.ExpiredAt,
                ExpiryAt = trainingRequest.ExpiryAt,
                RemoveAt = trainingRequest.RemoveAt,
                Regions = trainingRequest.Regions,
                ProviderResponses = trainingRequest.ProviderResponses
            };
        }

        public async Task<CancelTrainingRequestViewModel> GetCancelTrainingRequestViewModel(Guid employerRequestId, string hashedAccountId)
        {
            var trainingRequest = await _mediator.Send(new GetTrainingRequestQuery { EmployerRequestId = employerRequestId, IncludeProviders = false });
            if (trainingRequest == null)
            {
                throw new ArgumentException($"The training request for {employerRequestId} was not found");
            }

            return new CancelTrainingRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                EmployerRequestId = trainingRequest.EmployerRequestId,
                StandardTitle = trainingRequest.StandardTitle,
                StandardLevel = trainingRequest.StandardLevel,
                NumberOfApprentices = trainingRequest.NumberOfApprentices,
                SameLocation = trainingRequest.SameLocation,
                SingleLocation = trainingRequest.SingleLocation,
                AtApprenticesWorkplace = trainingRequest.AtApprenticesWorkplace,
                DayRelease = trainingRequest.DayRelease,
                BlockRelease = trainingRequest.BlockRelease,
                Status = trainingRequest.Status,
                Regions = trainingRequest.Regions
            };
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

        public async Task<bool> HasExistingEmployerRequest(long accountId, string standardId)
        {
            var standard = await _mediator.Send(new GetStandardQuery(standardId));
            if (standard == null)
            {
                throw new ArgumentException($"The standard {standardId} was not found");
            }

            var existing = await _mediator.Send(new GetExistingEmployerRequestQuery { AccountId = accountId, StandardReference = standard.IfateReferenceNumber });
            return existing;
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
                var newNumberOfApprentices = int.Parse(viewModel.NumberOfApprentices);

                if(employerRequest.NumberOfApprentices != newNumberOfApprentices)
                {
                    if (employerRequest.NumberOfApprentices == 1 && newNumberOfApprentices > 1)
                    {
                        employerRequest.SameLocation = "Yes";
                    }
                    else if (employerRequest.NumberOfApprentices > 1 && newNumberOfApprentices == 1)
                    {
                        employerRequest.SameLocation = null;
                        employerRequest.Regions = null;
                    }

                    viewModel.BackToCheckAnswers = false;
                    employerRequest.NumberOfApprentices = int.Parse(viewModel.NumberOfApprentices);
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
                var newSameLocation = viewModel.SameLocation;

                if (employerRequest.SameLocation != newSameLocation)
                {
                    if (newSameLocation == "No")
                    {
                        employerRequest.SingleLocation = null;
                    }
                    else if (newSameLocation == "Yes")
                    {
                        employerRequest.Regions = null;
                        employerRequest.SingleLocation = viewModel.Location;
                    }

                    viewModel.BackToCheckAnswers = false;
                    employerRequest.SameLocation = viewModel.SameLocation;
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
                // this is a special case where the attempted value will not automatically populate the input element as the input element
                // is being replaced with an autocomplete using javascript
                SingleLocation = modelState.GetAttemptedValueWhenInvalid(nameof(EnterSingleLocationEmployerRequestViewModel.SingleLocation), string.Empty, SessionEmployerRequest.SingleLocation),
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
                // this is a special case where the attempted value will not automatically populate the input elements as the input elements are
                // radio buttons which are dynamically created from a list
                MultipleLocations = modelState.GetAttemptedValueWhenInvalid(nameof(EnterMultipleLocationsEmployerRequestViewModel.MultipleLocations), [], SessionEmployerRequest.Regions?.Select(r => r.Id.ToString()).ToArray() ?? [])
            };
        }

        public async Task<bool> ValidateEnterMultipleLocationsEmployerRequestViewModel(EnterMultipleLocationsEmployerRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            return await ValidateViewModel(_employerRequestOrchestratorValidators.EnterMultipleLocationsEmployerRequestViewModelValidator, viewModel, modelState);
        }

        public async Task UpdateMultipleLocationsForEmployerRequest(EnterMultipleLocationsEmployerRequestViewModel viewModel)
        {
            var regions = await _mediator.Send(new GetRegionsQuery());

            UpdateSessionEmployerRequest((employerRequest) =>
            {
                employerRequest.Regions = viewModel.MultipleLocations
                    .Select(s =>
                    {
                        var matchingRegion = regions.FirstOrDefault(r => r.Id == int.Parse(s));
                        return matchingRegion != null
                            ? new Region
                            {
                                Id = matchingRegion.Id,
                                SubregionName = matchingRegion.SubregionName,
                                RegionName = matchingRegion.RegionName
                            }
                            : null;
                    })
                    .Where(r => r != null)
                    .ToList();
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
                SameLocation = employerRequest.SameLocation,
                SingleLocation = employerRequest.SingleLocation,
                MultipleLocations = employerRequest.Regions?.Select(r => r.Id.ToString()).ToArray(),
                AtApprenticesWorkplace = employerRequest.AtApprenticesWorkplace,
                DayRelease = employerRequest.DayRelease,
                BlockRelease = employerRequest.BlockRelease,
                Regions = employerRequest.Regions
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
                SameLocation = viewModel.SameLocation,
                SingleLocation = viewModel.SingleLocation,
                MultipleLocations = viewModel.MultipleLocations,
                AtApprenticesWorkplace = viewModel.AtApprenticesWorkplace,
                DayRelease = viewModel.DayRelease,
                BlockRelease = viewModel.BlockRelease,
                RequestedBy = GetCurrentUserId,
                ModifiedBy = GetCurrentUserId
            });

            ClearSessionEmployerRequest();

            return employerRequestId;
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

        public async Task<SubmitConfirmationEmployerRequestViewModel> GetSubmitConfirmationEmployerRequestViewModel(string hashedAccountId, Guid employerRequestId)
        {
            var result = await _mediator.Send(new GetSubmitEmployerRequestConfirmationQuery { EmployerRequestId = employerRequestId });
            if(result == null)
            {
                throw new ArgumentException($"The employer request {employerRequestId} was not found");
            }

            return new SubmitConfirmationEmployerRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                StandardTitle = result.StandardTitle,
                StandardLevel = result.StandardLevel,
                NumberOfApprentices = result.NumberOfApprentices.ToString(),
                SameLocation = result.SameLocation,
                SingleLocation = result.SingleLocation,
                AtApprenticesWorkplace = result.AtApprenticesWorkplace,
                DayRelease = result.DayRelease,
                BlockRelease = result.BlockRelease,
                RequestedByEmail = result.RequestedByEmail,
                FindApprenticeshipTrainingCoursesUrl = $"{_config.FindApprenticeshipTrainingBaseUrl}courses",
                ExpiryAfterMonths = result.ExpiryAfterMonths,
                Regions = result.Regions
            };
        }
    }
}