using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class EnterSameLocationEmployerRequestViewModelValidatorTests
    {
        private EnterSameLocationEmployerRequestViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EnterSameLocationEmployerRequestViewModelValidator();
        }

        [Test]
        public void ShouldHaveError_WhenSameLocationIsEmpty()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                SameLocation = string.Empty
            };

            // Act
            var result = _validator.TestValidate(viewModel);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SameLocation)
                .WithErrorMessage("You must make a selection");
        }

        [Test]
        public void ShouldNotHaveError_WhenSameLocationIsNotEmpty()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                SameLocation = "true"
            };

            // Act
            var result = _validator.TestValidate(viewModel);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SameLocation);
        }
    }
}
