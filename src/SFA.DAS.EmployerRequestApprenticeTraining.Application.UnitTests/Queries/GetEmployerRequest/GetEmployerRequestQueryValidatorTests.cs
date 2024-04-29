using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequest
{
    [TestFixture]
    public class GetEmployerRequestsQueryValidatorTests
    {
        private GetEmployerRequestQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetEmployerRequestQueryValidator();
        }

        [Test]
        public void Validate_ShouldHaveValidationError_WhenEmployerRequestIdIsEmpty()
        {
            // Arrange
            var query = new GetEmployerRequestQuery { EmployerRequestId = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmployerRequestId)
                  .WithErrorMessage("'Employer Request Id' must not be empty.");
        }

        [Test]
        public void Validate_ShouldNotHaveValidationError_WhenEmployerRequestIdIsValid()
        {
            // Arrange
            var query = new GetEmployerRequestQuery { EmployerRequestId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.EmployerRequestId);
        }
    }
}
