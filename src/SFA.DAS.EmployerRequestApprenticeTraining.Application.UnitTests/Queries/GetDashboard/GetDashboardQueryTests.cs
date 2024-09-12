using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetDashboard;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerRequestApprenticeTraining.UnitTests.Application.Queries.GetDashboard
{
    [TestFixture]
    public class GetDashboardQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private GetDashboardQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _handler = new GetDashboardQueryHandler(_outerApiMock.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Dashboard(
            Dashboard dashboard)
        {
            // Arrange
            var accountId = 123;

            _outerApiMock
                .Setup(x => x.GetDashboard(accountId))
                .ReturnsAsync(dashboard);

            var query = new GetDashboardQuery { AccountId = accountId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _outerApiMock.Verify(x => x.GetDashboard(accountId), Times.Once);

            result.Should().NotBeNull();
            result.AggregatedEmployerRequests.Should().BeEquivalentTo(dashboard.AggregatedEmployerRequests);
            result.ExpiryAfterMonths.Should().Be(dashboard.ExpiryAfterMonths);
        }
    }
}