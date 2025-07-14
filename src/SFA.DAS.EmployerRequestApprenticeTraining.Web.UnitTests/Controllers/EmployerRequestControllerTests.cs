using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerRequestControllerTests
    {
        private Mock<IEmployerRequestOrchestrator> _orchestratorMock;
        private EmployerRequestController _sut;

        [SetUp]
        public void Setup()
        {
            _orchestratorMock = new Mock<IEmployerRequestOrchestrator>();

            _sut = new EmployerRequestController(_orchestratorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _sut?.Dispose();
        }

        [Test]
        public async Task Dashboard_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new Parameters
            {
                HashedAccountId = "ABC123",
                AccountId = 123
            };

            var viewModel = new DashboardViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.GetDashboardViewModel(parameters.AccountId, parameters.HashedAccountId)).ReturnsAsync(viewModel);

            // Act
            var result = await _sut.Dashboard(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task ViewTrainingRequest_ShouldAcknowledgeProviderResponsesAndRedirectToViewTrainingRequest()
        {
            // Arrange
            var parameters = new ViewTrainingRequestParameters
            {
                HashedAccountId = "ABC123",
                EmployerRequestId = Guid.NewGuid()
            };

            var viewModel = new ViewTrainingRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.GetViewTrainingRequestViewModel(parameters.EmployerRequestId, parameters.HashedAccountId)).ReturnsAsync(viewModel);

            // Act
            var result = await _sut.ViewTrainingRequest(parameters) as ViewResult;

            // Assert
            _orchestratorMock.Verify(o => o.AcknowledgeProviderResponses(parameters.EmployerRequestId), Times.Once);

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task CancelTrainingRequest_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new CancelTrainingRequestParameters
            {
                HashedAccountId = "ABC123",
                AccountId = 123
            };

            var viewModel = new CancelTrainingRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.GetCancelTrainingRequestViewModel(parameters.EmployerRequestId, parameters.HashedAccountId)).ReturnsAsync(viewModel);

            // Act
            var result = await _sut.CancelTrainingRequest(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task CancelTrainingRequest_ShouldCancelRequestAndRedirectToCancelConfirmation()
        {
            // Arrange
            var viewModel = new CancelTrainingRequestViewModel
            {
                HashedAccountId = "ABC123",
                EmployerRequestId = Guid.NewGuid()
            };

            // Act
            var result = await _sut.CancelTrainingRequest(viewModel) as RedirectToRouteResult;

            // Assert
            _orchestratorMock.Verify(o => o.CancelTrainingRequest(viewModel.EmployerRequestId, viewModel.HashedAccountId), Times.Once);

            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CancelConfirmationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["employerRequestId"].Should().Be(viewModel.EmployerRequestId);
        }

        [Test]
        public async Task CancelConfirmation_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            var viewModel = new CancelConfirmationEmployerRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                CancelledByEmail = "test@test.com"
            };

            _orchestratorMock
                .Setup(o => o.GetCancelConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId))
                .ReturnsAsync(viewModel);

            // Act
            var result = await _sut.CancelConfirmation(hashedAccountId, employerRequestId) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }
        [Test]
        public async Task Overview_ShouldCall_GetStandardAndStartSession()
        {
            // Arrange
            var parameters = new OverviewParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "123",
                Location = "London"
            };

            var standardModel = new Standard
            {
                StandardLevel = 1,
                StandardReference = "ST0123",
                StandardSector = "Sector A",
                StandardTitle = "Standard A",
            };

            _orchestratorMock.Setup(o => o.GetStandardAndStartSession(parameters)).ReturnsAsync(standardModel);

            // Act
            await _sut.Overview(parameters);

            // Assert
            _orchestratorMock.Verify(o => o.GetStandardAndStartSession(parameters), Times.Once);
        }

        [Test]
        public async Task Overview_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new OverviewParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "123",
                Location = "London"
            };

            var standardModel = new Standard
            {
                StandardLevel = 1,
                StandardReference = "ST0123",
                StandardSector = "Sector A",
                StandardTitle = "Standard A",
            };

            var viewModel = new OverviewEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardReference = "ST0123",
                Location = "London",
                StandardTitle = standardModel.StandardTitle,
                StandardLevel = standardModel.StandardLevel,
                StandardLarsCode = 123,
                FindApprenticeshipTrainingBaseUrl = "http:///www.thsite.com"
            };

            _orchestratorMock.Setup(o => o.GetStandardAndStartSession(parameters)).ReturnsAsync(standardModel);
            _orchestratorMock.Setup(o => o.GetOverviewEmployerRequestViewModel(parameters)).Returns(viewModel);

            // Act
            var result = await _sut.Overview(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task Overview_ShouldRedirectToExisting_WhenEmployerRequestExists()
        {
            // Arrange
            var parameters = new OverviewParameters
            {
                AccountId = 10022856,
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "1234",
                Location = "London"
            };

            var standardModel = new Standard
            {
                StandardLevel = 1,
                StandardReference = "ST0001",
                StandardSector = "Sector A",
                StandardTitle = "Standard A",
            };

            _orchestratorMock.Setup(o => o.GetStandardAndStartSession(parameters)).ReturnsAsync(standardModel);
            _orchestratorMock.Setup(o => o.HasExistingEmployerRequest(parameters.AccountId, standardModel.StandardReference)).ReturnsAsync(true);

            // Act
            var result = await _sut.Overview(parameters) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.ExistingEmployerRequestRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(parameters.HashedAccountId);
        }

        [Test]
        public void EnterApprentices_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123"
            };

            var viewModel = new EnterApprenticesEmployerRequestViewModel();

            _orchestratorMock.Setup(o => o.GetEnterApprenticesEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>())).Returns(viewModel);

            // Act
            var result = _sut.EnterApprentices(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldRedirectToEnterApprenticesWhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterApprenticesRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldCallUpdateNumberOfApprenticesForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            await _sut.EnterApprentices(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateNumberOfApprenticesForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsValidAndBackToCheckAnswersIsFalseAndOneApprentice()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false,
                NumberOfApprentices = "1"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldRedirectToEnterSameLocationWhenModelStateIsValidAndBackToCheckAnswersIsFalseAndMultipleApprentices()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false,
                NumberOfApprentices = "2"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSameLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldRedirectToCheckYourAnswersWhenModelStateIsValidAndBackToCheckAnswersIsTrue()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = true
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public void EnterSameLocation_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123"
            };

            var viewModel = new EnterSameLocationEmployerRequestViewModel();

            _orchestratorMock.Setup(o => o.GetEnterSameLocationEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>())).Returns(viewModel);

            // Act
            var result = _sut.EnterSameLocation(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task EnterSameLocationPost_ShouldRedirectToEnterSameLocationWhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EnterSameLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSameLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterSameLocationPost_ShouldCallUpdateSameLocationForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            await _sut.EnterSameLocation(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateSameLocationForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterSameLocationPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsValidAndBackToCheckAnswersIsFalseAndSameLocationIsYes()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false,
                SameLocation = "Yes"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSameLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterSameLocationPost_ShouldRedirectToEnterMultipleLocationsWhenModelStateIsValidAndBackToCheckAnswersIsFalseAndSameLocationIsNo()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false,
                SameLocation = "No"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSameLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterMultipleLocationsRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterSameLocationPost_ShouldRedirectToCheckYourAnswersWhenModelStateIsValidAndBackToCheckAnswersIsTrue()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = true
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSameLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterMultipleLocations_Get_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123"
            };
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel();

            _orchestratorMock
                .Setup(o => o.GetEnterMultipleLocationsEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(viewModel);

            // Act
            var result = await _sut.EnterMultipleLocations(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task EnterMultipleLocations_Post_ShouldRedirectToEnterMultipleLocations_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock
                .Setup(o => o.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.EnterMultipleLocations(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterMultipleLocationsRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterMultipleLocations_Post_ShouldCallUpdateMultipleLocationsForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock
                .Setup(o => o.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(true);

            // Act
            await _sut.EnterMultipleLocations(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateMultipleLocationsForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterMultipleLocations_Post_ShouldRedirectToEnterTrainingOptionsWhenModelStateIsValidAndBackToCheckAnswersIsFalse()
        {
            // Arrange
            var viewModel = new EnterMultipleLocationsEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false
            };

            _orchestratorMock
                .Setup(o => o.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.EnterMultipleLocations(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterTrainingOptionsRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public void EnterSingleLocation_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123"
            };

            var viewModel = new EnterSingleLocationEmployerRequestViewModel();

            _orchestratorMock.Setup(o => o.GetEnterSingleLocationEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>())).Returns(viewModel);

            // Act
            var result = _sut.EnterSingleLocation(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EnterSingleLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldCallUpdateSingleLocationForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            await _sut.EnterSingleLocation(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateSingleLocationForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsValidAndBackToCheckAnswersIsFalse()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = false
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSingleLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterTrainingOptionsRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldRedirectToCheckYourAnswersWhenModelStateIsValidAndBackToCheckAnswersIsTrue()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                BackToCheckAnswers = true
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSingleLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public void EnterTrainingOptions_Get_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123"
            };
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel();

            _orchestratorMock
                .Setup(o => o.GetEnterTrainingOptionsEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>()))
                .Returns(viewModel);

            // Act
            var result = _sut.EnterTrainingOptions(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task EnterTrainingOptions_Post_ShouldRedirectToEnterTrainingOptions_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
            };

            _orchestratorMock
                .Setup(o => o.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.EnterTrainingOptions(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterTrainingOptionsRouteGet);
            result.RouteValues["HashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task EnterTrainingOptions_Post_ShouldUpdateTrainingOptionsAndRedirect_WhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterTrainingOptionsEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock
                .Setup(o => o.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.EnterTrainingOptions(viewModel) as RedirectToRouteResult;

            // Assert
            _orchestratorMock.Verify(o => o.UpdateTrainingOptionsForEmployerRequest(viewModel), Times.Once);

            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
            result.RouteValues["HashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public void CheckYourAnswers_Get_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new SubmitEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
            };
            var viewModel = new CheckYourAnswersEmployerRequestViewModel();

            _orchestratorMock
                .Setup(o => o.GetCheckYourAnswersEmployerRequestViewModel(parameters, It.IsAny<ModelStateDictionary>()))
                .Returns(viewModel);

            // Act
            var result = _sut.CheckYourAnswers(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task CheckYourAnswers_Post_ShouldRedirectToExisting_WhenEmployerRequestExists()
        {
            // Arrange
            var viewModel = new CheckYourAnswersEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                AccountId = 123
            };

            _orchestratorMock.Setup(o => o.HasExistingEmployerRequest(viewModel.AccountId, It.IsAny<string>())).ReturnsAsync(true);
            _orchestratorMock.Setup(o => o.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.CheckYourAnswers(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.ExistingEmployerRequestRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }


        [Test]
        public async Task CheckYourAnswers_Post_ShouldRedirectToCheckYourAnswers_WhenModelStateIsInvalid()
        {
            // Arrange
            var viewModel = new CheckYourAnswersEmployerRequestViewModel
            {
                HashedAccountId = "ABC123"
            };

            _orchestratorMock
                .Setup(o => o.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.CheckYourAnswers(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
            result.RouteValues["HashedAccountId"].Should().Be(viewModel.HashedAccountId);
        }

        [Test]
        public async Task CheckYourAnswers_Post_ShouldRedirectToSubmitConfirmation_WhenEmployerRequestDoesNotExist_AndModelStateIsValid()
        {
            // Arrange
            var viewModel = new CheckYourAnswersEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                StandardReference = "ST0002",
                AccountId = 123
            };

            _orchestratorMock.Setup(o => o.HasExistingEmployerRequest(viewModel.AccountId, viewModel.StandardReference)).ReturnsAsync(false);
            _orchestratorMock
                .Setup(o => o.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(true);

            var employerRequestId = Guid.NewGuid();
            _orchestratorMock
                .Setup(o => o.SubmitEmployerRequest(viewModel))
                .ReturnsAsync(employerRequestId);

            // Act
            var result = await _sut.CheckYourAnswers(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.SubmitConfirmationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["employerRequestId"].Should().Be(employerRequestId);
        }


        [Test]
        public async Task SubmitConfirmation_Get_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var hashedAccountId = "ABC123";

            var viewModel = new SubmitConfirmationEmployerRequestViewModel
            {
                HashedAccountId = hashedAccountId,
                FindApprenticeshipTrainingCoursesUrl = "https://example.com/",
                RequestedByEmail = "test@example.com"
            };

            _orchestratorMock
                .Setup(o => o.GetSubmitConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId))
                .ReturnsAsync(viewModel);

            // Act
            var result = await _sut.SubmitConfirmation(hashedAccountId, employerRequestId) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }
    }
}
