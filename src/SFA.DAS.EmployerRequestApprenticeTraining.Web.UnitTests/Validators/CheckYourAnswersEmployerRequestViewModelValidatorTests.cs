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
    public class CheckYourAnswersEmployerRequestViewModelValidatorTests
    {
        private CheckYourAnswersEmployerRequestViewModelValidator _sut;
        private Mock<ILocationService> _locationServiceMock;

        [SetUp]
        public void SetUp()
        {
            _locationServiceMock = new Mock<ILocationService>();
            _sut = new CheckYourAnswersEmployerRequestViewModelValidator(_locationServiceMock.Object);
        }

        [Test]
        public async Task Should_Have_Error_When_NumberOfApprentices_Is_Empty()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { NumberOfApprentices = "" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.NumberOfApprentices);
        }

        [Test]
        public async Task Should_Not_Have_Error_When_NumberOfApprentices_Is_Valid()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { NumberOfApprentices = "10" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.NumberOfApprentices);
        }

        [Test]
        public async Task Should_Have_Error_When_SameLocation_Is_Empty_And_NumberOfApprentices_Greater_Than_1()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { NumberOfApprentices = "2", SameLocation = "" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.SameLocation);
        }

        [Test]
        public async Task Should_Not_Have_Error_When_SameLocation_Is_Yes_And_NumberOfApprentices_Greater_Than_1()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { NumberOfApprentices = "2", SameLocation = "Yes" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.SameLocation);
        }

        [Test]
        public async Task Should_Have_Error_When_SingleLocation_Is_Empty_And_SameLocation_Is_Yes()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { SameLocation = "Yes", SingleLocation = "" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.SingleLocation);
        }

        [Test]
        public async Task Should_Not_Have_Error_When_SingleLocation_Is_Valid_And_SameLocation_Is_Yes()
        {
            _locationServiceMock.Setup(x => x.CheckLocationExists(It.IsAny<string>())).ReturnsAsync(true);

            var model = new CheckYourAnswersEmployerRequestViewModel { SameLocation = "Yes", SingleLocation = "ValidLocation" };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.SingleLocation);
        }

        [Test]
        public async Task Should_Have_Error_When_MultipleLocations_Is_Empty_And_SameLocation_Is_No()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { SameLocation = "No", MultipleLocations = [] };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.MultipleLocations);
        }

        [Test]
        public async Task Should_Not_Have_Error_When_MultipleLocations_Is_Valid_And_SameLocation_Is_No()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { SameLocation = "No", MultipleLocations = ["Location1", "Location2"] };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.MultipleLocations);
        }

        [Test]
        public async Task Should_Have_Error_When_AtApprenticesWorkplace_Is_False_And_No_Other_TrainingOptions_Are_Selected()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { AtApprenticesWorkplace = false, DayRelease = false, BlockRelease = false };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.AtApprenticesWorkplace);
        }

        [Test]
        public async Task Should_Not_Have_Error_When_AtApprenticesWorkplace_Is_True()
        {
            var model = new CheckYourAnswersEmployerRequestViewModel { AtApprenticesWorkplace = true };
            var result = await _sut.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AtApprenticesWorkplace);
        }
    }
}
