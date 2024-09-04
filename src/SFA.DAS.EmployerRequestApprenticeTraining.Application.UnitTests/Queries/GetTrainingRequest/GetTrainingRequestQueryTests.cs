using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetTrainingRequest
{
    [TestFixture]
    public class GetTrainingRequestQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private Mock<IValidator<GetTrainingRequestQuery>> _mockValidator;
        private GetTrainingRequestQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockValidator = new Mock<IValidator<GetTrainingRequestQuery>>();
            _handler = new GetTrainingRequestQueryHandler(_outerApiMock.Object, _mockValidator.Object);
        }

        [Test]
        public async Task Handle_Should_CallValidator()
        {
            // Arrange
            var query = new GetTrainingRequestQuery { EmployerRequestId = Guid.NewGuid() };
            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ReturnTrainingRequest_When_EmployerRequestId_IsProvided()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var trainingRequest = new TrainingRequest() { EmployerRequestId = employerRequestId };
            var query = new GetTrainingRequestQuery { EmployerRequestId = employerRequestId };

            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _outerApiMock.Setup(api => api.GetTrainingRequest(employerRequestId)).ReturnsAsync(trainingRequest);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(trainingRequest);
        }

        [Test]
        public async Task Handle_Should_ReturnNull_When_NoMatchingEmployerRequestFound()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var query = new GetTrainingRequestQuery { EmployerRequestId = employerRequestId };

            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _outerApiMock.Setup(api => api.GetTrainingRequest(employerRequestId)).ReturnsAsync((TrainingRequest?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Handle_Should_ThrowValidationException_When_ValidationFails()
        {
            // Arrange
            var query = new GetTrainingRequestQuery();
            var validationResult = new ValidationResult();

            _mockValidator
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("Property", "Error") } ));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
