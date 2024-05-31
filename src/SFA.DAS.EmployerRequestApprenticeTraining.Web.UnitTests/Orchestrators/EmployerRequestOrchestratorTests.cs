using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class EmployerRequestOrchestratorTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ISessionStorageService> _sessionStorageMock;
        private Mock<IOptions<EmployerRequestApprenticeTrainingWebConfiguration>> _optionsMock;
        private EmployerRequestOrchestrator _sut;
        private EmployerRequestApprenticeTrainingWebConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _sessionStorageMock = new Mock<ISessionStorageService>();
            _config = new EmployerRequestApprenticeTrainingWebConfiguration
            {
                FindApprenticeshipTrainingBaseUrl = "http://example.com"
            };
            _optionsMock = new Mock<IOptions<EmployerRequestApprenticeTrainingWebConfiguration>>();
            _optionsMock.Setup(o => o.Value).Returns(_config);

            _sut = new EmployerRequestOrchestrator(_mediatorMock.Object, _sessionStorageMock.Object, _optionsMock.Object);
        }

        [Test]
        public async Task GetOverviewEmployerRequestViewModel_ShouldReturnViewModel_WhenStandardExists()
        {
            // Arrange
            var parameters = new OverviewEmployerRequestParameters
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
            var parameters = new OverviewEmployerRequestParameters
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
        public async Task GetViewEmployerRequestsViewModel_ShouldReturnViewModel_WhenRequestsExist()
        {
            // Arrange
            var accountId = 123;
            var employerRequests = new List<EmployerRequest> { new EmployerRequest { Id = Guid.NewGuid(), AccountId = accountId, RequestType = RequestType.Shortlist } };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployerRequestsQuery>(), default)).ReturnsAsync(employerRequests);

            // Act
            var result = await _sut.GetViewEmployerRequestsViewModel(accountId);

            // Assert
            result.Should().NotBeNull();
            result.EmployerRequests.Count.Should().Be(1);
            result.EmployerRequests[0].AccountId.Should().Be(accountId);
        }

        [Test]
        public async Task GetViewEmployerRequestViewModel_ShouldReturnViewModel_WhenRequestExists()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var employerRequest = new EmployerRequest { Id = employerRequestId, AccountId = 123, RequestType = RequestType.Shortlist };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployerRequestQuery>(), default)).ReturnsAsync(employerRequest);

            // Act
            var result = await _sut.GetViewEmployerRequestViewModel(employerRequestId);

            // Assert
            result.Should().NotBeNull();
            result.EmployerRequestId.Should().Be(employerRequest.Id);
            result.AccountId.Should().Be(employerRequest.AccountId);
            result.RequestType.Should().Be(employerRequest.RequestType);
        }

        [Test]
        public void StartEmployerRequest_ShouldSetEmployerRequestInSession()
        {
            // Act
            _sut.StartEmployerRequest();

            // Assert
            _sessionStorageMock.VerifySet(x => x.EmployerRequest = It.IsAny<EmployerRequest>(), Times.Once);
        }

        [Test]
        public async Task CreateEmployerRequest_ShouldReturnEmployerRequestId_WhenRequestIsCreated()
        {
            // Arrange
            var request = new CreateEmployerRequestViewModel
            {
                HashedAccountId = "ABC123",
                RequestType = RequestType.Shortlist
            };
            var employerRequestId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateEmployerRequestCommand>(), default)).ReturnsAsync(employerRequestId);

            // Act
            var result = await _sut.CreateEmployerRequest(request);

            // Assert
            result.Should().Be(employerRequestId);
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
    }
}
