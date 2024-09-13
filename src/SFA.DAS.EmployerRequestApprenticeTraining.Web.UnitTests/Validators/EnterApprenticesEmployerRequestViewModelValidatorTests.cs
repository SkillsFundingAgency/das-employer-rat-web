using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class EnterApprenticesEmployerRequestViewModelValidatorTests
    {
        private EnterApprenticesEmployerRequestViewModelValidator _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new EnterApprenticesEmployerRequestViewModelValidator();
        }

        [TestCase("5")]
        [TestCase("1000")]
        [TestCase("9999")]
        public void Should_Not_Have_Error_When_NumberOfApprentices_Is_Valid(string validNumber)
        {
            // Arrange
            var model = new EnterApprenticesEmployerRequestViewModel { NumberOfApprentices = validNumber };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.NumberOfApprentices);
        }

        [Test]
        public void Should_Have_Error_When_NumberOfApprentices_Is_Empty()
        {
            // Arrange
            var model = new EnterApprenticesEmployerRequestViewModel { NumberOfApprentices = string.Empty };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NumberOfApprentices)
                  .WithErrorMessage("Enter the number of apprentices");
        }

        [TestCase("abc")]
        [TestCase("!@#")]
        public void Should_Have_Error_When_NumberOfApprentices_Is_Not_A_Valid_Number(string invalidNumber)
        {
            // Arrange
            var model = new EnterApprenticesEmployerRequestViewModel { NumberOfApprentices = invalidNumber };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NumberOfApprentices)
                  .WithErrorMessage("Enter a number");
        }

        [TestCase("0")]
        [TestCase("-1")]
        [TestCase("10000")]
        public void Should_Have_Error_When_NumberOfApprentices_Is_Out_Of_Range(string outOfRangeNumber)
        {
            // Arrange
            var model = new EnterApprenticesEmployerRequestViewModel { NumberOfApprentices = outOfRangeNumber };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NumberOfApprentices)
                  .WithErrorMessage("Enter a number between 1 and 9999");
        }

        [TestCase("5.5")]
        [TestCase("1000.1")]
        [TestCase("9999.9")]
        public void Should_Have_Error_When_NumberOfApprentices_Is_Not_A_Whole_Number(string invalidNumber)
        {
            // Arrange
            var model = new EnterApprenticesEmployerRequestViewModel { NumberOfApprentices = invalidNumber };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NumberOfApprentices)
                  .WithErrorMessage("Enter a whole number");
        }
    }
}
