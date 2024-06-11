using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
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
        public async Task Overview_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            var viewModel = new OverviewEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.GetOverviewEmployerRequestViewModel(parameters)).ReturnsAsync(viewModel);

            // Act
            var result = await _sut.Overview(parameters) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Test]
        public async Task Start_ShouldCallStartEmployerRequest()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            // Act
            await _sut.Start(parameters);

            // Assert
            _orchestratorMock.Verify(o => o.StartEmployerRequest(parameters.Location), Times.Once);
        }

        [Test]
        public async Task Start_ShouldRedirectToOverview()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            // Act
            var result = await _sut.Start(parameters) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterApprenticesRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(parameters.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(parameters.RequestType);
            result.RouteValues["standardId"].Should().Be(parameters.StandardId);
            result.RouteValues["location"].Should().Be(parameters.Location);
        }

        [Test]
        public async Task Cancel_ShouldCallStartEmployerRequest()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            // Act
            await _sut.Cancel(parameters);

            // Assert
            _orchestratorMock.Verify(o => o.StartEmployerRequest(parameters.Location), Times.Once);
        }

        [Test]
        public async Task Cancel_ShouldRedirectToOverview()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            // Act
            var result = await _sut.Cancel(parameters) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.OverviewEmployerRequestRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(parameters.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(parameters.RequestType);
            result.RouteValues["standardId"].Should().Be(parameters.StandardId);
            result.RouteValues["location"].Should().Be(parameters.Location);
        }

        [Test]
        public void EnterApprentices_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
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
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterApprenticesRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(viewModel.RequestType);
            result.RouteValues["standardId"].Should().Be(viewModel.StandardId);
            result.RouteValues["location"].Should().Be(viewModel.Location);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldCallUpdateNumberOfApprenticesForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            await _sut.EnterApprentices(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateNumberOfApprenticesForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterApprenticesPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterApprenticesEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterApprentices(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(viewModel.RequestType);
            result.RouteValues["standardId"].Should().Be(viewModel.StandardId);
            result.RouteValues["location"].Should().Be(viewModel.Location);
        }

        [Test]
        public void EnterSingleLocation_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var parameters = new CreateEmployerRequestParameters
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
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
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(false);

            // Act
            var result = await _sut.EnterSingleLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(viewModel.RequestType);
            result.RouteValues["standardId"].Should().Be(viewModel.StandardId);
            result.RouteValues["location"].Should().Be(viewModel.Location);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldCallUpdateSingleLocationForEmployerRequestWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            await _sut.EnterSingleLocation(viewModel);

            // Assert
            _orchestratorMock.Verify(o => o.UpdateSingleLocationForEmployerRequest(viewModel), Times.Once);
        }

        [Test]
        public async Task EnterSingleLocationPost_ShouldRedirectToEnterSingleLocationWhenModelStateIsValid()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist,
                StandardId = "ST0123",
                Location = "London"
            };

            _orchestratorMock.Setup(o => o.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, It.IsAny<ModelStateDictionary>())).ReturnsAsync(true);

            // Act
            var result = await _sut.EnterSingleLocation(viewModel) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.EnterSingleLocationRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(viewModel.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(viewModel.RequestType);
            result.RouteValues["standardId"].Should().Be(viewModel.StandardId);
            result.RouteValues["location"].Should().Be(viewModel.Location);
        }
    }
}
