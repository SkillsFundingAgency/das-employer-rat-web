using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class EnterMultipleLocationsEmployerRequestViewModelValidatorTests
    {
        private EnterMultipleLocationsEmployerRequestViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EnterMultipleLocationsEmployerRequestViewModelValidator();
        }

        [Test]
        public void Should_Have_Error_When_MultipleLocations_Is_Null()
        {
            // Arrange
            var model = new EnterMultipleLocationsEmployerRequestViewModel
            {
                MultipleLocations = null
            };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MultipleLocations)
                .WithErrorMessage("Select a location");
        }

        [Test]
        public void Should_Have_Error_When_MultipleLocations_Is_Empty()
        {
            // Arrange
            var model = new EnterMultipleLocationsEmployerRequestViewModel
            {
                MultipleLocations = new string[] { }
            };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MultipleLocations)
                .WithErrorMessage("Select a location");
        }

        [Test]
        public void Should_Not_Have_Error_When_MultipleLocations_Is_Valid()
        {
            // Arrange
            var model = new EnterMultipleLocationsEmployerRequestViewModel
            {
                MultipleLocations = new[] { "1", "2" }
            };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.MultipleLocations);
        }
    }
}
