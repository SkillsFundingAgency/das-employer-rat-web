using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.TestHelper.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.UnitTests.Services.Locations
{
    [TestFixture]
    public class LocationServiceTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private Mock<ILogger<LocationService>> _loggerMock;
        private LocationService _locationService;

        [SetUp]
        public void SetUp()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _loggerMock = new Mock<ILogger<LocationService>>();
            _locationService = new LocationService(_outerApiMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetLocations_ShouldReturnLocations_WhenApiCallIsSuccessful()
        {
            // Arrange
            var searchTerm = "London";
            var locations = new List<LocationSearchResponse>
            {
                new LocationSearchResponse { Name = "London" },
                new LocationSearchResponse { Name = "London Fields " }
            };

            _outerApiMock.Setup(x => x.GetLocations(searchTerm, false)).ReturnsAsync(locations);

            // Act
            var result = await _locationService.GetLocations(searchTerm, false);

            // Assert
            result.Should().BeEquivalentTo(locations);
        }

        [Test]
        public async Task GetLocations_ShouldReturnEmptyList_AndLogError_WhenApiCallFails()
        {
            // Arrange
            var searchTerm = "London";

            _outerApiMock.Setup(p => p.GetLocations(searchTerm, false))
                .Throws(new ApiException(new HttpRequestMessage(), new HttpResponseMessage(), string.Empty));

            // Act
            var result = await _locationService.GetLocations(searchTerm, false);

            // Assert
            result.Should().BeEquivalentTo(new List<LocationSearchResponse>());
            _loggerMock.VerifyLogError($"Unable to get locations for searchTerm:{searchTerm}", Times.Once);
        }

        [Test]
        public async Task CheckLocationExists_ShouldReturnFalse_WhenSearchTermIsNullOrEmpty()
        {
            // Act
            var result = await _locationService.CheckLocationExists(string.Empty);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task CheckLocationExists_ShouldReturnFalse_WhenSearchTermIsLessThanThreeCharacters()
        {
            // Act
            var result = await _locationService.CheckLocationExists("Lo");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task CheckLocationExists_ShouldReturnTrue_WhenLocationExists()
        {
            // Arrange
            var searchTerm = "London";
            var locations = new List<LocationSearchResponse>
            {
                new LocationSearchResponse { Name = "London" }
            };

            _outerApiMock.Setup(x => x.GetLocations("London", true)).ReturnsAsync(locations);

            // Act
            var result = await _locationService.CheckLocationExists(searchTerm);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task CheckLocationExists_ShouldReturnFalse_WhenLocationDoesNotExist()
        {
            // Arrange
            var searchTerm = "London";
            var locations = new List<LocationSearchResponse>
            {
            };

            _outerApiMock.Setup(x => x.GetLocations("London", true)).ReturnsAsync(locations);

            // Act
            var result = await _locationService.CheckLocationExists(searchTerm);

            // Assert
            result.Should().BeFalse();
        }
    }
}
