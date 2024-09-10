using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.AcknowledgeProviderResponses;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetDashboard;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CancelEmployerRequest;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Configuration;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class EmployerRequestOrchestratorTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ISessionStorageService> _sessionStorageMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IValidator<EnterApprenticesEmployerRequestViewModel>> _enterApprenticesEmployerRequestViewModelValidatorMock;
        private Mock<IValidator<EnterSameLocationEmployerRequestViewModel>> _enterSameLocationEmployerRequestViewModelValidatorMock;
        private Mock<IValidator<EnterSingleLocationEmployerRequestViewModel>> _enterSingleLocationEmployerRequestViewModelValidatorMock;
        private Mock<IValidator<EnterMultipleLocationsEmployerRequestViewModel>> _enterMultipleLocationsEmployerRequestViewModelValidatorMock;
        private Mock<IValidator<EnterTrainingOptionsEmployerRequestViewModel>> _enterTrainingOptionsEmployerRequestViewModelValidatorMock;
        private Mock<IValidator<CheckYourAnswersEmployerRequestViewModel>> _checkYourAnswersEmployerRequestViewModelValidatorMock;
        private Mock<IOptions<EmployerRequestApprenticeTrainingWebConfiguration>> _optionsMock;
        private EmployerRequestOrchestrator _sut;
        private EmployerRequestApprenticeTrainingWebConfiguration _config;
        private UrlBuilder _urlBuilder;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _sessionStorageMock = new Mock<ISessionStorageService>();
            _locationServiceMock = new Mock<ILocationService>();
            _userServiceMock = new Mock<IUserService>();
            _urlBuilder = new UrlBuilder("LOCAL");

            _enterApprenticesEmployerRequestViewModelValidatorMock = new Mock<IValidator<EnterApprenticesEmployerRequestViewModel>>();
            _enterSameLocationEmployerRequestViewModelValidatorMock = new Mock<IValidator<EnterSameLocationEmployerRequestViewModel>>();
            _enterSingleLocationEmployerRequestViewModelValidatorMock = new Mock<IValidator<EnterSingleLocationEmployerRequestViewModel>>();
            _enterMultipleLocationsEmployerRequestViewModelValidatorMock = new Mock<IValidator<EnterMultipleLocationsEmployerRequestViewModel>>();
            _enterTrainingOptionsEmployerRequestViewModelValidatorMock = new Mock<IValidator<EnterTrainingOptionsEmployerRequestViewModel>>();
            _checkYourAnswersEmployerRequestViewModelValidatorMock = new Mock<IValidator<CheckYourAnswersEmployerRequestViewModel>>();

            var employerRequestOrchestratorValidators = new EmployerRequestOrchestratorValidators
            {
                EnterApprenticesEmployerRequestViewModelValidator = _enterApprenticesEmployerRequestViewModelValidatorMock.Object,
                EnterSameLocationEmployerRequestViewModelValidator = _enterSameLocationEmployerRequestViewModelValidatorMock.Object,
                EnterSingleLocationEmployerRequestViewModelValidator = _enterSingleLocationEmployerRequestViewModelValidatorMock.Object,
                EnterMultipleLocationsEmployerRequestViewModelValidator = _enterMultipleLocationsEmployerRequestViewModelValidatorMock.Object,
                EnterTrainingOptionsEmployerRequestViewModelValidator = _enterTrainingOptionsEmployerRequestViewModelValidatorMock.Object,
                CheckYourAnswersEmployerRequestViewModelValidator = _checkYourAnswersEmployerRequestViewModelValidatorMock.Object
            };
            
            _config = new EmployerRequestApprenticeTrainingWebConfiguration
            {
                FindApprenticeshipTrainingBaseUrl = "http://example.com"
            };
            _optionsMock = new Mock<IOptions<EmployerRequestApprenticeTrainingWebConfiguration>>();
            _optionsMock.Setup(o => o.Value).Returns(_config);

            _sut = new EmployerRequestOrchestrator(_mediatorMock.Object, _sessionStorageMock.Object, 
                _locationServiceMock.Object,  _userServiceMock.Object, 
                employerRequestOrchestratorValidators, _optionsMock.Object,
                _urlBuilder);
        }

        [Test, MoqAutoData]
        public async Task GetDashboardViewModel_ShouldReturnDashboardViewModel_WithCorrectValues(Dashboard dashboard)
        {
            // Arrange
            var accountId = 123;
            var hashedAccountId = "ABC123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDashboardQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dashboard);

            // Act
            var result = await _sut.GetDashboardViewModel(accountId, hashedAccountId);

            // Assert
            result.Should().NotBeNull();
            result.Dashboard.Should().Be(dashboard);
            result.HashedAccountId.Should().Be(hashedAccountId);
            result.FindApprenticeshipTrainingCoursesUrl.Should().Be($"{_config.FindApprenticeshipTrainingBaseUrl}courses");
            result.EmployerAccountDashboardUrl.Should().Be(_urlBuilder.AccountsLink("AccountsHome", hashedAccountId));
        }

        [Test]
        public async Task GetViewTrainingRequestViewModel_ShouldReturnViewModel_WhenTrainingRequestExists()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            var trainingRequest = new TrainingRequest
            {
                EmployerRequestId = employerRequestId,
                StandardTitle = "StandardTitle",
                StandardLevel = 3,
                NumberOfApprentices = 5,
                SameLocation = "Yes",
                SingleLocation = "Location",
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = false,
                RequestedAt = DateTime.Now,
                Status = RequestStatus.Active,
                ExpiredAt = DateTime.Now.AddMonths(1),
                ExpiryAt = DateTime.Now.AddMonths(2),
                RemoveAt = DateTime.Now.AddMonths(3),
                Regions = new List<Region>
                {
                    new Region { Id = 1, RegionName = "Region1", SubregionName = "Subregion1" }
                },
                ProviderResponses = new List<ProviderResponse>
                {
                    new ProviderResponse { ContactName = "Some Person", Email = "sperson@somewhere.com", PhoneNumber = "12345", ProviderName = "PROVIDER", RespondedAt = DateTime.Now, Ukprn = 10003030, Website = "www.somewhere.com" },
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrainingRequestQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(trainingRequest);

            // Act
            var result = await _sut.GetViewTrainingRequestViewModel(employerRequestId, hashedAccountId);

            // Assert
            result.Should().NotBeNull();
            result.EmployerRequestId.Should().Be(trainingRequest.EmployerRequestId);
            result.StandardTitle.Should().Be(trainingRequest.StandardTitle);
            result.StandardLevel.Should().Be(trainingRequest.StandardLevel);
            result.NumberOfApprentices.Should().Be(trainingRequest.NumberOfApprentices);
            result.SameLocation.Should().Be(trainingRequest.SameLocation);
            result.SingleLocation.Should().Be(trainingRequest.SingleLocation);
            result.AtApprenticesWorkplace.Should().Be(trainingRequest.AtApprenticesWorkplace);
            result.DayRelease.Should().Be(trainingRequest.DayRelease);
            result.BlockRelease.Should().Be(trainingRequest.BlockRelease);
            result.RequestedAt.Should().Be(trainingRequest.RequestedAt);
            result.Status.Should().Be(trainingRequest.Status);
            result.ExpiredAt.Should().Be(trainingRequest.ExpiredAt);
            result.ExpiryAt.Should().Be(trainingRequest.ExpiryAt);
            result.RemoveAt.Should().Be(trainingRequest.RemoveAt);
            result.Regions.Should().BeEquivalentTo(trainingRequest.Regions);
            result.ProviderResponses.Should().BeEquivalentTo(trainingRequest.ProviderResponses);
        }

        [Test]
        public async Task GetViewTrainingRequestViewModel_ShouldThrowArgumentException_WhenTrainingRequestDoesNotExist()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrainingRequestQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((TrainingRequest)null);

            // Act
            Func<Task> act = async () => await _sut.GetViewTrainingRequestViewModel(employerRequestId, hashedAccountId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage($"The training request for {employerRequestId} was not found");
        }

        [Test]
        public async Task CancelTrainingRequest_ShouldCallMediatorWithCorrectCommandAndCancelledBy()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";
            var userId = Guid.NewGuid();

            _userServiceMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

            // Act
            await _sut.CancelTrainingRequest(employerRequestId, hashedAccountId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<CancelEmployerRequestCommand>(cmd =>
                cmd.EmployerRequestId == employerRequestId && cmd.CancelledBy == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCancelTrainingRequestViewModel_ShouldReturnViewModel_WhenTrainingRequestExists()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";
            var trainingRequest = new TrainingRequest
            {
                EmployerRequestId = employerRequestId,
                StandardTitle = "StandardTitle",
                StandardLevel = 3,
                NumberOfApprentices = 5,
                SameLocation = "Yes",
                SingleLocation = "Location",
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = false,
                Status = RequestStatus.Active,
                Regions = new List<Region>
            {
                new Region { Id = 1, RegionName = "Region1", SubregionName = "Subregion1" }
            }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrainingRequestQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(trainingRequest);

            // Act
            var result = await _sut.GetCancelTrainingRequestViewModel(employerRequestId, hashedAccountId);

            // Assert
            result.Should().NotBeNull();
            result.EmployerRequestId.Should().Be(trainingRequest.EmployerRequestId);
            result.StandardTitle.Should().Be(trainingRequest.StandardTitle);
            result.StandardLevel.Should().Be(trainingRequest.StandardLevel);
            result.NumberOfApprentices.Should().Be(trainingRequest.NumberOfApprentices);
            result.SameLocation.Should().Be(trainingRequest.SameLocation);
            result.SingleLocation.Should().Be(trainingRequest.SingleLocation);
            result.AtApprenticesWorkplace.Should().Be(trainingRequest.AtApprenticesWorkplace);
            result.DayRelease.Should().Be(trainingRequest.DayRelease);
            result.BlockRelease.Should().Be(trainingRequest.BlockRelease);
            result.Status.Should().Be(trainingRequest.Status);
            result.Regions.Should().BeEquivalentTo(trainingRequest.Regions);
        }

        [Test]
        public async Task GetCancelTrainingRequestViewModel_ShouldThrowArgumentException_WhenTrainingRequestDoesNotExist()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrainingRequestQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((TrainingRequest)null);

            // Act
            Func<Task> act = async () => await _sut.GetCancelTrainingRequestViewModel(employerRequestId, hashedAccountId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage($"The training request for {employerRequestId} was not found");
        }


        [Test]
        public async Task AcknowledgeProviderResponses_ShouldCallMediator_WithCorrectParameters()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(u => u.GetUserId()).Returns(userId.ToString());

            // Act
            await _sut.AcknowledgeProviderResponses(employerRequestId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<AcknowledgeProviderResponsesCommand>(cmd =>
                cmd.EmployerRequestId == employerRequestId && cmd.AcknowledgedBy == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOverviewEmployerRequestViewModel_ShouldReturnViewModel_WhenStandardExists()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };
            
            var standard = new Standard { Title = "Title", Level = 3, LarsCode = 123 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStandardQuery>(), default)).ReturnsAsync(standard);

            // Act
            var result = await _sut.GetOverviewEmployerRequestViewModel(parameters);

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.StandardId.Should().Be(parameters.StandardId);
            result.Location.Should().Be(parameters.Location);
            result.StandardTitle.Should().Be(standard.Title);
            result.StandardLevel.Should().Be(standard.Level);
            result.StandardLarsCode.Should().Be(standard.LarsCode);
            result.FindApprenticeshipTrainingBaseUrl.Should().Be(_config.FindApprenticeshipTrainingBaseUrl);
        }

        [Test]
        public void GetOverviewEmployerRequestViewModel_ShouldThrowArgumentException_WhenStandardDoesNotExist()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStandardQuery>(), default)).ReturnsAsync((Standard)null);

            // Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetOverviewEmployerRequestViewModel(parameters));

            // Assert
            ex.Message.Should().Be($"The standard {parameters.StandardId} was not found");
        }

        [Test]
        public async Task HasExistingEmployerRequest_ShouldReturnTrue_WhenEmployerRequestExists()
        {
            // Arrange
            var accountId = 12345;
            var standardId = "ST0123";
            var standard = new Standard { IfateReferenceNumber = standardId };

            _mediatorMock.Setup(m => m.Send(It.Is<GetStandardQuery>(p => p.StandardId == standardId), default)).ReturnsAsync(standard);
            _mediatorMock.Setup(m => m.Send(It.Is<GetExistingEmployerRequestQuery>(p => p.AccountId == accountId && p.StandardReference == standardId), default)).ReturnsAsync(true);

            // Act
            var result = await _sut.HasExistingEmployerRequest(accountId, standardId);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task HasExistingEmployerRequest_ShouldReturnFalse_WhenEmployerRequestDoesNotExist()
        {
            // Arrange
            var accountId = 12345;
            var standardId = "ST0123";
            var standard = new Standard { IfateReferenceNumber = standardId };

            _mediatorMock.Setup(m => m.Send(It.Is<GetStandardQuery>(p => p.StandardId == standardId), default)).ReturnsAsync(standard);
            _mediatorMock.Setup(m => m.Send(It.Is<GetExistingEmployerRequestQuery>(p => p.AccountId == accountId && p.StandardReference == standardId), default)).ReturnsAsync(false);

            // Act
            var result = await _sut.HasExistingEmployerRequest(accountId, standardId);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasExistingEmployerRequest_ShouldThrowArgumentException_WhenStandardDoesNotExist()
        {
            // Arrange
            var accountId = 123;
            var standardId = "ST0123";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStandardQuery>(), default)).ReturnsAsync((Standard)null);

            // Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _sut.HasExistingEmployerRequest(accountId, standardId));

            // Assert
            ex.Message.Should().Be($"The standard {standardId} was not found");
        }

        [Test]
        public async Task StartEmployerRequest_ShouldSetEmployerRequestInSession()
        {
            // Act
            await _sut.StartEmployerRequest("Some Location");

            // Assert
            _sessionStorageMock.VerifySet(x => x.EmployerRequest = It.IsAny<EmployerRequest>(), Times.Once);
        }

        [Test]
        public void GetEnterApprenticesEmployerRequestViewModel_ShouldReturnViewModel_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London",
                BackToCheckAnswers = false
            };
            var employerRequest = new EmployerRequest { NumberOfApprentices = 5 };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            var result = _sut.GetEnterApprenticesEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.BackToCheckAnswers.Should().BeFalse();
            result.NumberOfApprentices.Should().Be(employerRequest.NumberOfApprentices.ToString());
        }

        [Test]
        public void GetEnterApprenticesEmployerRequestViewModel_ShouldReturnViewModel_WhenSessionIsEmpty()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London",
                BackToCheckAnswers = true
            };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            var result = _sut.GetEnterApprenticesEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.BackToCheckAnswers.Should().BeTrue();
            result.NumberOfApprentices.Should().Be(string.Empty);
        }

        [Test]
        public async Task ValidateEnterApprenticesEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _enterApprenticesEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateEnterApprenticesEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PropertyName", "Error message")
            });

            _enterApprenticesEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["PropertyName"].Errors[0].ErrorMessage.Should().Be("Error message");
        }

        [Test]
        public void UpdateNumberOfApprenticesForEmployerRequest_ShouldUpdateNumberOfApprentices_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                NumberOfApprentices = "1"
            };
            var employerRequest = new EmployerRequest { SameLocation = "Yes" };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            _sut.UpdateNumberOfApprenticesForEmployerRequest(viewModel);

            // Assert
            employerRequest.NumberOfApprentices.Should().Be(1);
            employerRequest.SameLocation.Should().Be("Yes");
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = employerRequest, Times.Once);
        }

        [Test]
        public void UpdateNumberOfApprenticesForEmployerRequest_ShouldSetNewEmployerRequest_WhenSessionIsEmpty()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                NumberOfApprentices = "1"
            };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            _sut.UpdateNumberOfApprenticesForEmployerRequest(viewModel);

            // Assert
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = It.Is<EmployerRequest>(er => er.NumberOfApprentices == 1 && er.SameLocation == null), Times.Once);
        }


        [Test]
        public void GetEnterSameLocationEmployerRequestViewModel_ShouldReturnViewModel_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };
            var employerRequest = new EmployerRequest { SameLocation = "Yes" };
            var modelState = new ModelStateDictionary();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            var result = _sut.GetEnterSameLocationEmployerRequestViewModel(parameters, modelState);

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.SameLocation.Should().Be(employerRequest.SameLocation);
        }

        [Test]
        public async Task ValidateEnterSameLocationEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _enterSameLocationEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateEnterSameLocationEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PropertyName", "Error message")
            });

            _enterSameLocationEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["PropertyName"].Errors[0].ErrorMessage.Should().Be("Error message");
        }

        [Test]
        public void UpdateSameLocationForEmployerRequest_ShouldUpdateSameLocation_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                SameLocation = "Yes"
            };
            var employerRequest = new EmployerRequest();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            _sut.UpdateSameLocationForEmployerRequest(viewModel);

            // Assert
            employerRequest.SameLocation.Should().Be("Yes");
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = employerRequest, Times.Once);
        }

        [Test]
        public void UpdateSameLocationForEmployerRequest_ShouldSetNewEmployerRequest_WhenSessionIsEmpty()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                SameLocation = "Yes"
            };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            _sut.UpdateSameLocationForEmployerRequest(viewModel);

            // Assert
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = It.Is<EmployerRequest>(er => er.SameLocation == "Yes"), Times.Once);
        }

        [Test]
        public async Task GetEnterMultipleLocationsEmployerRequestViewModel_ShouldReturnViewModel_WithClosestRegion_WhenLocationIsProvided()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            var regions = new List<Region>
            {
                new Region { Id = 1, RegionName = "Region1", SubregionName = "SubregionName1" },
                new Region { Id = 2, RegionName = "Region1", SubregionName = "SubregionName2" }
            };

            var closestRegion = new Region { Id = 1, RegionName = "Region1", SubregionName = "SubregionName1" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRegionsQuery>(), default)).ReturnsAsync(regions);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetClosestRegionQuery>(), default)).ReturnsAsync(closestRegion);

            var employerRequest = new EmployerRequest();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            var result = await _sut.GetEnterMultipleLocationsEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.SubregionsGroupedByRegions.Should().HaveCount(1);
            result.MultipleLocations.Should().Contain(closestRegion.Id.ToString());
        }

        [Test]
        public async Task ValidateEnterMultipleLocationsEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _enterMultipleLocationsEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateEnterMultipleLocationsEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PropertyName", "Error message")
            });

            _enterMultipleLocationsEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["PropertyName"].Errors[0].ErrorMessage.Should().Be("Error message");
        }


        [Test]
        public async Task GetEnterMultipleLocationsEmployerRequestViewModel_ShouldReturnViewModel_WithoutClosestRegion_WhenLocationIsNotProvided()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123"
            };

            var regions = new List<Region>
            {
                new Region { Id = 1, RegionName = "Region1", SubregionName = "SubregionName1" },
                new Region { Id = 2, RegionName = "Region1", SubregionName = "SubregionName2" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRegionsQuery>(), default)).ReturnsAsync(regions);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetClosestRegionQuery>(), default)).ReturnsAsync((Region)null);

            // Act
            var result = await _sut.GetEnterMultipleLocationsEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.SubregionsGroupedByRegions.Should().HaveCount(1);
            result.MultipleLocations.Should().HaveCount(0);
        }

        [Test]
        public async Task UpdateMultipleLocationsForEmployerRequest_ShouldUpdateMultipleLocations_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel
            {
                MultipleLocations = new[] { "1", "2" }
            };
            var employerRequest = new EmployerRequest();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Region> { new Region { Id = 1 }, new Region { Id = 2 } });

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            await _sut.UpdateMultipleLocationsForEmployerRequest(viewModel);

            // Assert
            employerRequest.Regions.Should().NotBeNull();
            employerRequest.Regions.Count.Should().Be(2);
            employerRequest.Regions[0].Id.Should().Be(1);
            employerRequest.Regions[1].Id.Should().Be(2);
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = employerRequest, Times.Once);
        }

        [Test]
        public async Task UpdateMultipleLocationsForEmployerRequest_ShouldSetNewEmployerRequest_WhenSessionIsEmpty()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel
            {
                MultipleLocations = new[] { "1", "2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRegionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Region> { new Region { Id = 1 }, new Region { Id = 2 } });

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            await _sut.UpdateMultipleLocationsForEmployerRequest(viewModel);

            // Assert
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = It.Is<EmployerRequest>(er => er.Regions.Count == 2 && er.Regions[0].Id == 1 && er.Regions[1].Id == 2), Times.Once);
        }


        [Test]
        public async Task SubmitEmployerRequest_ShouldReturnEmployerRequestId_And_ClearSession_WhenRequestIsCreated()
        {
            // Arrange
            var request = new CheckYourAnswersEmployerRequestViewModel
            {
                Location = string.Empty,
                RequestType = RequestType.Shortlist,
                HashedAccountId = "ABC123",
                AccountId = 12345,
                StandardTitle = "Title",
                StandardId = "123",
                StandardLevel = 3,
                NumberOfApprentices = "1",
                AtApprenticesWorkplace = true,
                DayRelease = false,
                BlockRelease = false,
                SingleLocation = "Telford, Shropshire"
            };
            var employerRequestId = Guid.NewGuid();

            var standard = new Standard { Title = "Title", Level = 3, LarsCode = 123, IfateReferenceNumber = "ST0222" };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetStandardQuery>(), default))
                .ReturnsAsync(standard);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SubmitEmployerRequestCommand>(), default))
                .ReturnsAsync(employerRequestId);

            _userServiceMock
                .Setup(m => m.GetUserId())
                .Returns(Guid.NewGuid().ToString());

            // Act
            var result = await _sut.SubmitEmployerRequest(request);

            // Assert
            result.Should().Be(employerRequestId);
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = null, Times.Once);
        }


        [Test]
        public void BackLink_Should_Return_Shortlist_Url_When_RequestType_Is_Shortlist()
        {
            // Arrange
            var model = new OverviewEmployerRequestViewModel
            {
                RequestType = RequestType.Shortlist,
                FindApprenticeshipTrainingBaseUrl = "http://example.com/"
            };

            // Act
            var backLink = model.BackLink;

            // Assert
            backLink.Should().Be("http://example.com/shortlist");
        }

        [Test]
        public void BackLink_Should_Return_ProviderSearch_Url_When_RequestType_Is_ProviderSearch()
        {
            // Arrange
            var model = new OverviewEmployerRequestViewModel
            {
                RequestType = RequestType.Providers,
                FindApprenticeshipTrainingBaseUrl = "http://example.com/",
                StandardLarsCode = 123,
                Location = "London"
            };

            // Act
            var backLink = model.BackLink;

            // Assert
            backLink.Should().Be("http://example.com/courses/123/providers?Location=London");
        }

        [Test]
        public void GetEnterSingleLocationEmployerRequestViewModel_ShouldReturnViewModel_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };
            var employerRequest = new EmployerRequest { SingleLocation = "Location1" };
            var modelState = new ModelStateDictionary();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            var result = _sut.GetEnterSingleLocationEmployerRequestViewModel(parameters, modelState);

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.SingleLocation.Should().Be(employerRequest.SingleLocation);
        }

        [Test]
        public void GetEnterSingleLocationEmployerRequestViewModel_ShouldReturnViewModel_WhenSessionIsEmpty()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London",
                BackToCheckAnswers = true

            };
            var modelState = new ModelStateDictionary();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            var result = _sut.GetEnterSingleLocationEmployerRequestViewModel(parameters, modelState);

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.BackToCheckAnswers.Should().BeTrue();
            result.SingleLocation.Should().BeNull();
        }

        [Test]
        public async Task ValidateEnterSingleLocationEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _enterSingleLocationEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateEnterSingleLocationEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PropertyName", "Error message")
            });

            _enterSingleLocationEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["PropertyName"].Errors[0].ErrorMessage.Should().Be("Error message");
        }

        [Test]
        public void UpdateSingleLocationForEmployerRequest_ShouldUpdateSingleLocation_WhenSessionHasEmployerRequest()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                SingleLocation = "New Location"
            };
            var employerRequest = new EmployerRequest();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            _sut.UpdateSingleLocationForEmployerRequest(viewModel);

            // Assert
            employerRequest.SingleLocation.Should().Be("New Location");
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = employerRequest, Times.Once);
        }

        [Test]
        public void UpdateSingleLocationForEmployerRequest_ShouldSetNewEmployerRequest_WhenSessionIsEmpty()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                SingleLocation = "New Location"
            };

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns((EmployerRequest)null);

            // Act
            _sut.UpdateSingleLocationForEmployerRequest(viewModel);

            // Assert
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = It.Is<EmployerRequest>(er => er.SingleLocation == "New Location"), Times.Once);
        }

        [Test]
        public void GetEnterTrainingOptionsEmployerRequestViewModel_ShouldReturnViewModel_WithCorrectValues()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London",
                BackToCheckAnswers = true
            };
            var employerRequest = new EmployerRequest
            {
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = false
            };
            _sessionStorageMock.Setup(x => x.EmployerRequest).Returns(employerRequest);

            // Act
            var result = _sut.GetEnterTrainingOptionsEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.BackToCheckAnswers.Should().BeTrue();
            result.AtApprenticesWorkplace.Should().BeTrue();
            result.DayRelease.Should().BeTrue();
            result.BlockRelease.Should().BeFalse();
        }

        [Test]
        public async Task ValidateEnterTrainingOptionsEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _enterTrainingOptionsEmployerRequestViewModelValidatorMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateEnterTrainingOptionsEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new[] { new ValidationFailure("AtApprenticesWorkplace", "Select a training option") });

            _enterTrainingOptionsEmployerRequestViewModelValidatorMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["AtApprenticesWorkplace"].Errors[0].ErrorMessage.Should().Be("Select a training option");
        }

        [Test]
        public void UpdateTrainingOptionsForEmployerRequest_ShouldUpdateEmployerRequest_WithCorrectValues()
        {
            // Arrange
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = true,
                DayRelease = false,
                BlockRelease = true
            };
            var employerRequest = new EmployerRequest();

            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            _sut.UpdateTrainingOptionsForEmployerRequest(viewModel);

            // Assert
            employerRequest.AtApprenticesWorkplace.Should().BeTrue();
            employerRequest.DayRelease.Should().BeFalse();
            employerRequest.BlockRelease.Should().BeTrue();
            _sessionStorageMock.VerifySet(s => s.EmployerRequest = employerRequest, Times.Once);
        }

        [Test]
        public async Task GetCheckYourAnswersEmployerRequestViewModel_ShouldReturnViewModel_WhenStandardExists()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            var standard = new Standard { Title = "Title", Level = 3, LarsCode = 123 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStandardQuery>(), default)).ReturnsAsync(standard);
            var employerRequest = new EmployerRequest
            {
                NumberOfApprentices = 5,
                SingleLocation = "Location1",
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = false
            };
            _sessionStorageMock.Setup(s => s.EmployerRequest).Returns(employerRequest);

            // Act
            var result = await _sut.GetCheckYourAnswersEmployerRequestViewModel(parameters, new ModelStateDictionary());

            // Assert
            result.Should().NotBeNull();
            result.HashedAccountId.Should().Be(parameters.HashedAccountId);
            result.StandardId.Should().Be(parameters.StandardId);
            result.RequestType.Should().Be(parameters.RequestType);
            result.Location.Should().Be(parameters.Location);
            result.StandardTitle.Should().Be(standard.Title);
            result.StandardLevel.Should().Be(standard.Level);
            result.NumberOfApprentices.Should().Be(employerRequest.NumberOfApprentices.ToString());
            result.SingleLocation.Should().Be(employerRequest.SingleLocation);
            result.AtApprenticesWorkplace.Should().BeTrue();
            result.DayRelease.Should().BeTrue();
            result.BlockRelease.Should().BeFalse();
        }

        [Test]
        public async Task ValidateCheckYourAnswersEmployerRequestViewModel_ShouldReturnTrue_WhenModelIsValid()
        {
            // Arrange
            var viewModel = new CheckYourAnswersEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(); // No errors

            _checkYourAnswersEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeTrue();
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task ValidateCheckYourAnswersEmployerRequestViewModel_ShouldReturnFalse_WhenModelIsInvalid()
        {
            // Arrange
            var viewModel = new CheckYourAnswersEmployerRequestViewModel();
            var modelState = new ModelStateDictionary();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("PropertyName", "Error message")
            });

            _checkYourAnswersEmployerRequestViewModelValidatorMock
                .Setup(v => v.ValidateAsync(viewModel, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _sut.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, modelState);

            // Assert
            result.Should().BeFalse();
            modelState.IsValid.Should().BeFalse();
            modelState["PropertyName"].Errors[0].ErrorMessage.Should().Be("Error message");
        }

        [Test]
        public async Task GetSubmitConfirmationEmployerRequestViewModel_ShouldReturnViewModel_WhenRequestExists()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            var confirmation = new SubmitEmployerRequestConfirmation
            {
                StandardTitle = "StandardTitle",
                StandardLevel = 3,
                NumberOfApprentices = 5,
                SingleLocation = "Location",
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = false,
                RequestedByEmail = "test@example.com"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSubmitEmployerRequestConfirmationQuery>(), default)).ReturnsAsync(confirmation);

            // Act
            var result = await _sut.GetSubmitConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId);

            // Assert
            result.Should().NotBeNull();
            result.StandardTitle.Should().Be(confirmation.StandardTitle);
            result.StandardLevel.Should().Be(confirmation.StandardLevel);
            result.NumberOfApprentices.Should().Be(confirmation.NumberOfApprentices.ToString());
            result.SingleLocation.Should().Be(confirmation.SingleLocation);
            result.AtApprenticesWorkplace.Should().Be(confirmation.AtApprenticesWorkplace);
            result.DayRelease.Should().Be(confirmation.DayRelease);
            result.BlockRelease.Should().Be(confirmation.BlockRelease);
            result.RequestedByEmail.Should().Be(confirmation.RequestedByEmail);
            result.FindApprenticeshipTrainingCoursesUrl.Should().Be($"{_config.FindApprenticeshipTrainingBaseUrl}courses");
        }

        [Test]
        public void GetSubmitConfirmationEmployerRequestViewModel_ShouldThrowArgumentException_WhenRequestDoesNotExist()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSubmitEmployerRequestConfirmationQuery>(), default))
                .ReturnsAsync((SubmitEmployerRequestConfirmation)null);

            // Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetSubmitConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId));

            // Assert
            ex.Message.Should().Be($"The employer request {employerRequestId} was not found");
        }
    }
}
