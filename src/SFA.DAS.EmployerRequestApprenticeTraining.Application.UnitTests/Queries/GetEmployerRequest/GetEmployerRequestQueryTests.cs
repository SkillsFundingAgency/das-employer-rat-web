﻿using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequest
{
    [TestFixture]
    public class GetEmployerRequestQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private Mock<IValidator<GetEmployerRequestQuery>> _mockValidator;
        private GetEmployerRequestQueryHandler _handler;
        private GetEmployerRequestQuery _query;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockValidator = new Mock<IValidator<GetEmployerRequestQuery>>();
            _handler = new GetEmployerRequestQueryHandler(_mockOuterApi.Object, _mockValidator.Object);
            _query = new GetEmployerRequestQuery { EmployerRequestId = Guid.NewGuid() };
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
            var expectedRequest = new EmployerRequest
            {
                Id = _query.EmployerRequestId,
                AccountId = 123456,
                RequestType = RequestType.Shortlist
            };

            _mockOuterApi.Setup(x => x.GetEmployerRequest(_query.EmployerRequestId))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedRequest);
            _mockOuterApi.Verify(x => x.GetEmployerRequest(_query.EmployerRequestId), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.GetEmployerRequest(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}
