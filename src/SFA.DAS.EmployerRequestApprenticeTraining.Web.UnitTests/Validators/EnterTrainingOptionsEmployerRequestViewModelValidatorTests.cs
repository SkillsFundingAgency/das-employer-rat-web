using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class EnterTrainingOptionsEmployerRequestViewModelValidatorTests
    {
        private EnterTrainingOptionsEmployerRequestViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new EnterTrainingOptionsEmployerRequestViewModelValidator();
        }

        [Test]
        public void Should_Have_Error_When_No_Options_Selected()
        {
            // Arrange
            var model = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = false,
                BlockRelease = false
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AtApprenticesWorkplace).WithErrorMessage("Select a training option");
        }

        [Test]
        public void Should_Not_Have_Error_When_AtApprenticesWorkplace_Is_Selected()
        {
            // Arrange
            var model = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = true,
                DayRelease = false,
                BlockRelease = false
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [Test]
        public void Should_Not_Have_Error_When_DayRelease_Is_Selected()
        {
            // Arrange
            var model = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = true,
                BlockRelease = false
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [Test]
        public void Should_Not_Have_Error_When_BlockRelease_Is_Selected()
        {
            // Arrange
            var model = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = false,
                BlockRelease = true
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Options_Are_Selected()
        {
            // Arrange
            var model = new EnterTrainingOptionsEmployerRequestViewModel
            {
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = true
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
