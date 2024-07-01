using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Location;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Controllers
{
    [TestFixture]
    public class LocationsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private LocationsController _sut;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new LocationsController(_mediatorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _sut?.Dispose();
        }

        [Test]
        public async Task Locations_ShouldReturnJsonResult_WithLocationsViewModel()
        {
            // Arrange
            var searchTerm = "London";
            var locationList = new List<LocationSearchResponse>
            {
                new LocationSearchResponse { Name = "London" },
                new LocationSearchResponse { Name = "London Fields" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(locationList);

            // Act
            var result = await _sut.Locations(searchTerm) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<LocationsViewModel>();

            var model = result.Value as LocationsViewModel;
            model.Should().NotBeNull();
            model.Locations.Should().HaveCount(2);
            model.Locations.First().Name.Should().Be("London");
            model.Locations.Last().Name.Should().Be("London Fields");
        }

        [Test]
        public async Task Locations_ShouldReturnEmptyLocations_WhenQueryResultIsNull()
        {
            // Arrange
            var searchTerm = "NonExistentLocation";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<LocationSearchResponse>)null);

            // Act
            var result = await _sut.Locations(searchTerm) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<LocationsViewModel>();

            var model = result.Value as LocationsViewModel;
            model.Should().NotBeNull();
            model.Locations.Should().BeEmpty();
        }

        [Test]
        public async Task Locations_ShouldReturnEmptyLocations_WhenQueryResultIsEmpty()
        {
            // Arrange
            var searchTerm = "NonExistentLocation";
            var locationList = new List<LocationSearchResponse>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetLocationsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(locationList);

            // Act
            var result = await _sut.Locations(searchTerm) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<LocationsViewModel>();

            var model = result.Value as LocationsViewModel;
            model.Should().NotBeNull();
            model.Locations.Should().BeEmpty();
        }
    }
}
