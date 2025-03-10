﻿using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetLocations
{
    [TestFixture]
    public class GetLocationsQueryTests
    {
        private Mock<ILocationService> _mockLocationsService;
        private Mock<IValidator<GetLocationsQuery>> _mockValidator;
        private GetLocationsQueryHandler _handler;
        private GetLocationsQuery _query;

        [SetUp]
        public void Setup()
        {
            _mockLocationsService = new Mock<ILocationService>();
            _mockValidator = new Mock<IValidator<GetLocationsQuery>>();
            _handler = new GetLocationsQueryHandler(_mockLocationsService.Object, _mockValidator.Object);
            _query = new GetLocationsQuery("Lon");
        }

        [Test]
        public async Task Handle_ShouldCallValidator()
        {
            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            _mockValidator.Verify(x => x.ValidateAsync(_query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnLocations_WhenCalledWithValidSearchTerm()
        {
            // Arrange
            var expectedLocations = new List<LocationSearchResult>
            {
                new LocationSearchResult
                {
                    Name = "London"
                }
            };
            
            
            _mockLocationsService.Setup(x => x.GetLocations(_query.SearchTerm, false))
                .ReturnsAsync(expectedLocations);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedLocations);
            _mockLocationsService.Verify(x => x.GetLocations(_query.SearchTerm, false), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockLocationsService.Setup(x => x.GetLocations(It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}
