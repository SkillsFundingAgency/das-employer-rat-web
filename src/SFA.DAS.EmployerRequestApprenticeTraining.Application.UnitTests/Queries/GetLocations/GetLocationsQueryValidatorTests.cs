using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetLocations
{
    [TestFixture]
    public class GetLocationsQueryValidatorTests
    {
        private GetLocationsQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetLocationsQueryValidator();
        }

        [Test]
        public void Validate_ShouldHaveValidationError_WhenSearchTermIsEmpty()
        {
            // Arrange
            var query = new GetLocationsQuery(string.Empty);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
                  .WithErrorMessage("'Search Term' must not be empty.");
        }

        [Test]
        public void Validate_ShouldNotHaveValidationError_WhenEmployerRequestIdIsValid()
        {
            // Arrange
            var query = new GetLocationsQuery("Lon");

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
        }
    }
}
