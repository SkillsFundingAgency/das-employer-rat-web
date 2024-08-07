﻿using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequests
{
    [TestFixture]
    public class GetEmployerRequestsQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private Mock<IValidator<GetEmployerRequestsQuery>> _mockValidator;
        private GetEmployerRequestsQueryHandler _handler;
        private GetEmployerRequestsQuery _query;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockValidator = new Mock<IValidator<GetEmployerRequestsQuery>>();
            _handler = new GetEmployerRequestsQueryHandler(_mockOuterApi.Object, _mockValidator.Object);
            _query = new GetEmployerRequestsQuery { AccountId = 123456 };
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
        public async Task Handle_ShouldReturnEmployerRequest_WhenCalledWithValidId()
        {
            // Arrange
            var expectedRequests = new List<EmployerRequest>
            {
                new EmployerRequest
                {
                    Id = Guid.NewGuid(),
                    AccountId = 123456,
                    RequestType = RequestType.Shortlist
                }
            };

            _mockOuterApi.Setup(x => x.GetEmployerRequests(_query.AccountId))
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedRequests);
            _mockOuterApi.Verify(x => x.GetEmployerRequests(_query.AccountId), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.GetEmployerRequests(It.IsAny<long>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}
