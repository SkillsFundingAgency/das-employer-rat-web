using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequest
{
    [TestFixture]
    public class GetEmployerRequestQueryTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private Mock<IValidator<GetEmployerRequestQuery>> _mockValidator;
        private GetEmployerRequestQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _mockValidator = new Mock<IValidator<GetEmployerRequestQuery>>();
            _handler = new GetEmployerRequestQueryHandler(_outerApiMock.Object, _mockValidator.Object);
        }

        [Test]
        public async Task Handle_Should_CallValidator()
        {
            // Arrange
            var query = new GetEmployerRequestQuery { EmployerRequestId = Guid.NewGuid() };
            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ReturnEmployerRequest_When_EmployerRequestId_IsProvided()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var employerRequest = new EmployerRequest();
            var query = new GetEmployerRequestQuery { EmployerRequestId = employerRequestId };

            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _outerApiMock.Setup(api => api.GetEmployerRequest(employerRequestId)).ReturnsAsync(employerRequest);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(employerRequest);
        }

        [Test]
        public async Task Handle_Should_ReturnEmployerRequest_When_AccountId_And_StandardReference_AreProvided()
        {
            // Arrange
            var accountId = 123;
            var standardReference = "ST0123";
            var employerRequest = new EmployerRequest();
            var query = new GetEmployerRequestQuery { AccountId = accountId, StandardReference = standardReference };

            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _outerApiMock.Setup(api => api.GetEmployerRequest(accountId, standardReference)).ReturnsAsync(employerRequest);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(employerRequest);
        }

        [Test]
        public async Task Handle_Should_ReturnNull_When_NoMatchingEmployerRequestFound()
        {
            // Arrange
            var employerRequestId = Guid.NewGuid();
            var query = new GetEmployerRequestQuery { EmployerRequestId = employerRequestId };

            _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _outerApiMock.Setup(api => api.GetEmployerRequest(employerRequestId)).ReturnsAsync((EmployerRequest?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Handle_Should_ThrowValidationException_When_ValidationFails()
        {
            // Arrange
            var query = new GetEmployerRequestQuery();
            var validationResult = new ValidationResult();

            _mockValidator
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(new[] { new ValidationFailure("Property", "Error") } ));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
