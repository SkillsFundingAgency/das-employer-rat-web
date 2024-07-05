using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;
using System.Net;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetClosestRegion
{
    public class GetClosestRegionQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private Mock<ICacheStorageService> _cacheStorageServiceMock;
        private GetClosestRegionQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _cacheStorageServiceMock = new Mock<ICacheStorageService>();
            _handler = new GetClosestRegionQueryHandler(_outerApiMock.Object, _cacheStorageServiceMock.Object);
        }

        [Test]
        public async Task Handle_Returns_Region_From_Cache_When_Found()
        {
            // Arrange
            var query = new GetClosestRegionQuery { Location = "TestLocation" };
            var region = new Region { Id = 1, SubregionName = "TestRegion" };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<Region>(It.IsAny<string>()))
                .ReturnsAsync(region);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(region);
            _outerApiMock.Verify(x => x.GetClosestRegion(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Handle_Returns_Region_From_Api_And_Caches_It_When_Not_Found_In_Cache()
        {
            // Arrange
            var query = new GetClosestRegionQuery { Location = "TestLocation" };
            var region = new Region { Id = 1, SubregionName = "TestRegion" };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<Region?>(It.IsAny<string>()))
                .ReturnsAsync((Region?)null);

            _outerApiMock
                .Setup(x => x.GetClosestRegion(It.IsAny<string>()))
                .ReturnsAsync(region);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(region);
            _cacheStorageServiceMock.Verify(x => x.SaveToCache(It.IsAny<string>(), region, 1), Times.Once);
        }

        [Test]
        public async Task Handle_Returns_Null_When_Region_Not_Found_In_Api()
        {
            // Arrange
            var query = new GetClosestRegionQuery { Location = "TestLocation" };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<Region?>(It.IsAny<string>()))
                .ReturnsAsync((Region?)null);

            _outerApiMock
                .Setup(x => x.GetClosestRegion(It.IsAny<string>()))
                .ThrowsAsync(new ApiException(new HttpRequestMessage(), new HttpResponseMessage(HttpStatusCode.NotFound), string.Empty));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Handle_Throws_Exception_When_Api_Call_Fails_With_Unexpected_Status()
        {
            // Arrange
            var query = new GetClosestRegionQuery { Location = "TestLocation" };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<Region?>(It.IsAny<string>()))
                .ReturnsAsync((Region?)null);

            _outerApiMock
                .Setup(x => x.GetClosestRegion(It.IsAny<string>()))
                .ThrowsAsync(new ApiException(new HttpRequestMessage(), new HttpResponseMessage(HttpStatusCode.InternalServerError), string.Empty));

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"The closest region for {query.Location} cannot be retrieved");
        }
    }
}
