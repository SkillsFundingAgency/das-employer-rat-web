﻿using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequests
{
    [TestFixture]
    public class GetStandardQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private Mock<ICacheStorageService> _mockCacheStorageService;
        private Mock<IValidator<GetStandardQuery>> _mockValidator;
        private GetStandardQueryHandler _handler;
        private GetStandardQuery _query;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockCacheStorageService = new Mock<ICacheStorageService>();
            _mockValidator = new Mock<IValidator<GetStandardQuery>>();
            _handler = new GetStandardQueryHandler(_mockOuterApi.Object, _mockCacheStorageService.Object, _mockValidator.Object);
            _query = new GetStandardQuery("ST0100");
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
        public async Task Handle_ShouldCacheAndReturnStandard_WhenCalledWithValidId_ForFirstTime()
        {
            // Arrange
            var standard = new StandardResponse
            {
                StandardUId = "ST0100_1.0",
                IfateReferenceNumber = "ST0100",
                LarsCode = 543,
                Level = 2,
                Title = "Interesting standard"
            };

            _mockOuterApi
                .Setup(x => x.GetStandard(_query.StandardId))
                .ReturnsAsync(standard);

            _mockCacheStorageService
                .Setup(x => x.RetrieveFromCache<StandardResponse?>(It.IsAny<string>()))
                .ReturnsAsync((StandardResponse?)null);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(standard);
            _mockOuterApi.Verify(x => x.GetStandard(_query.StandardId), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnStandardFromCache_WhenCalledWithValidId_ForSecondTime()
        {
            // Arrange
            var standard = new StandardResponse
            {
                StandardUId = "ST0100_1.0",
                IfateReferenceNumber = "ST0100",
                LarsCode = 543,
                Level = 2,
                Title = "Interesting standard"
            };

            _mockCacheStorageService
                .Setup(x => x.RetrieveFromCache<StandardResponse?>($"GetStandard:{_query.StandardId}"))
                .ReturnsAsync(standard);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(standard);
            _mockOuterApi.Verify(x => x.GetStandard(_query.StandardId), Times.Never);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.GetStandard(It.IsAny<string>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}
