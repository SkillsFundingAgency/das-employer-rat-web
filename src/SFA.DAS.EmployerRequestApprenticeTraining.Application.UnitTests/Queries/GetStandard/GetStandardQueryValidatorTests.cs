using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetStandard
{
    [TestFixture]
    public class GetStandardQueryValidatorTests
    {
        private GetStandardQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetStandardQueryValidator();
        }

        [Test]
        public void Validate_ShouldHaveValidationError_WhenEmployerStandardIdIsEmpty()
        {
            // Arrange
            var query = new GetStandardQuery(string.Empty);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StandardId)
                  .WithErrorMessage("'Standard Id' must not be empty.");
        }

        [Test]
        public void Validate_ShouldNotHaveValidationError_WhenStandardIdIsValid()
        {
            // Arrange
            var query = new GetStandardQuery("ST0100");

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.StandardId);
        }
    }
}
