﻿using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequest
{
    [TestFixture]
    public class GetSubmitEmployerRequestConfirmationQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private Mock<IValidator<GetSubmitEmployerRequestConfirmationQuery>> _mockValidator;
        private GetSubmitEmployerRequestConfirmationQueryHandler _handler;
        private GetSubmitEmployerRequestConfirmationQuery _query;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockValidator = new Mock<IValidator<GetSubmitEmployerRequestConfirmationQuery>>();
            _handler = new GetSubmitEmployerRequestConfirmationQueryHandler(_mockOuterApi.Object, _mockValidator.Object);
            _query = new GetSubmitEmployerRequestConfirmationQuery { EmployerRequestId = Guid.NewGuid() };
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
            var expectedRequest = new SubmitEmployerRequestConfirmationResponse
            {
                EmployerRequestId = _query.EmployerRequestId,
                StandardTitle = "Title",
                StandardLevel = 1
            };

            _mockOuterApi.Setup(x => x.GetSubmitEmployerRequestConfirmation(_query.EmployerRequestId))
                .ReturnsAsync(expectedRequest);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedRequest);
            _mockOuterApi.Verify(x => x.GetSubmitEmployerRequestConfirmation(_query.EmployerRequestId), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.GetSubmitEmployerRequestConfirmation(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("API failure"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API failure");
        }
    }
}
