using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequests
{
    [TestFixture]
    public class GetEmployerRequestsQueryValidatorTests
    {
        private GetEmployerRequestsQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetEmployerRequestsQueryValidator();
        }

        [Test]
        public void Validate_ShouldHaveValidationError_WhenEmployerRequestIdIsEmpty()
        {
            // Arrange
            var query = new GetEmployerRequestsQuery { AccountId = 0 };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AccountId)
                  .WithErrorMessage("'Account Id' must not be empty.");
        }

        [Test]
        public void Validate_ShouldNotHaveValidationError_WhenEmployerRequestIdIsValid()
        {
            // Arrange
            var query = new GetEmployerRequestsQuery { AccountId = 123456 };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AccountId);
        }
    }
}
