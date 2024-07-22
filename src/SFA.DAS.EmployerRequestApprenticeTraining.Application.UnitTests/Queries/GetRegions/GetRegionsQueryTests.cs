using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;
using System.Net;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetRegions
{
    public class GetRegionsQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private Mock<ICacheStorageService> _cacheStorageServiceMock;
        private GetRegionsQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _cacheStorageServiceMock = new Mock<ICacheStorageService>();
            _handler = new GetRegionsQueryHandler(_outerApiMock.Object, _cacheStorageServiceMock.Object);
        }

        [Test]
        public async Task Handle_Returns_Regions_From_Cache_When_Found()
        {
            // Arrange
            var query = new GetRegionsQuery();
            var regions = new List<Region>
            {
                new Region { Id = 1, SubregionName = "Region1" },
                new Region { Id = 2, SubregionName = "Region2" }
            };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<List<Region>>(It.IsAny<string>()))
                .ReturnsAsync(regions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(regions);
            _outerApiMock.Verify(x => x.GetRegions(), Times.Never);
        }

        [Test]
        public async Task Handle_Returns_Regions_From_Api_And_Caches_It_When_Not_Found_In_Cache()
        {
            // Arrange
            var query = new GetRegionsQuery();
            var regions = new List<Region>
            {
                new Region { Id = 1, SubregionName = "Region1" },
                new Region { Id = 2, SubregionName = "Region2" }
            };

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<List<Region>?>(It.IsAny<string>()))
                .ReturnsAsync((List<Region>?)null);

            _outerApiMock
                .Setup(x => x.GetRegions())
                .ReturnsAsync(regions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(regions);
            _cacheStorageServiceMock.Verify(x => x.SaveToCache(It.IsAny<string>(), regions, 1), Times.Once);
        }

        [Test]
        public void Then_Throws_Exception_When_Api_Call_Fails()
        {
            // Arrange
            var query = new GetRegionsQuery();

            _cacheStorageServiceMock
                .Setup(x => x.RetrieveFromCache<List<Region>?>(It.IsAny<string>()))
                .ReturnsAsync((List<Region>?)null);

            _outerApiMock
                .Setup(x => x.GetRegions())
                .ThrowsAsync(new ApiException(new HttpRequestMessage(), new HttpResponseMessage(HttpStatusCode.InternalServerError), string.Empty));

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The regions cannot be retrieved");
        }
    }
}
