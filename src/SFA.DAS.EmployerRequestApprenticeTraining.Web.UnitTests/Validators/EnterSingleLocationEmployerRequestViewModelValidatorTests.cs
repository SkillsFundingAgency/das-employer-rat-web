using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class EnterSingleLocationEmployerRequestViewModelValidatorTests
    {
        private Mock<ILocationService> _locationServiceMock;
        private EnterSingleLocationEmployerRequestViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _locationServiceMock = new Mock<ILocationService>();
            _validator = new EnterSingleLocationEmployerRequestViewModelValidator(_locationServiceMock.Object);
        }

        [Test]
        public async Task Should_Have_Error_When_SingleLocation_Is_Empty()
        {
            // Arrange
            var model = new EnterSingleLocationEmployerRequestViewModel { SingleLocation = string.Empty };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SingleLocation).WithErrorMessage("Enter a town, city or postcode");
        }

        [Test]
        public async Task Should_Return_False_When_LocationName_Is_WhiteSpace()
        {
            // Arrange
            var model = new EnterSingleLocationEmployerRequestViewModel { SingleLocation = " " };

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SingleLocation).WithErrorMessage("Enter a town, city or postcode");
        }

        [Test]
        public async Task Should_Have_Error_When_SingleLocation_Is_Invalid()
        {
            // Arrange
            var model = new EnterSingleLocationEmployerRequestViewModel { SingleLocation = "InvalidLocation" };
            _locationServiceMock.Setup(x => x.CheckLocationExists(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SingleLocation).WithErrorMessage("Enter a valid location");
        }

        [Test]
        public async Task Should_Not_Have_Error_When_SingleLocation_Is_Valid()
        {
            // Arrange
            var model = new EnterSingleLocationEmployerRequestViewModel { SingleLocation = "ValidLocation" };
            _locationServiceMock.Setup(x => x.CheckLocationExists(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SingleLocation);
        }
    }
}
