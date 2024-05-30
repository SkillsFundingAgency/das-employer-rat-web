using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            var parameters = new OverviewEmployerRequestParameters
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
        public void Start_ShouldCallStartEmployerRequest()
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
            _sut.Start(parameters);

            // Assert
            _orchestratorMock.Verify(o => o.StartEmployerRequest(), Times.Once);
        }

        [Test]
        public void Start_ShouldRedirectToOverview()
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
            var result = _sut.Start(parameters) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteName.Should().Be(EmployerRequestController.OverviewEmployerRequestRouteGet);
            result.RouteValues["hashedAccountId"].Should().Be(parameters.HashedAccountId);
            result.RouteValues["requestType"].Should().Be(parameters.RequestType);
            result.RouteValues["standardId"].Should().Be(parameters.StandardId);
            result.RouteValues["location"].Should().Be(parameters.Location);
            result.RouteValues["BackToCheckAnswers"].Should().Be(false);
        }
    }
}
