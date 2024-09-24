using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Validators
{
    [TestFixture]
    public class OverviewEmployerRequestParametersValidatorTests
    {
        private OverviewEmployerRequestParametersValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new OverviewEmployerRequestParametersValidator();
        }

        [Test]
        public void Validate_Should_Have_Error_When_StandardId_Is_Empty()
        {
            // Arrange
            var model = new OverviewParameters { StandardId = string.Empty };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StandardId)
                .WithErrorMessage($"{ValidateRequiredQueryParametersAttribute.MissingRequireQueryParameterMessage}{nameof(OverviewParameters.StandardId)}");
        }

        [Test]
        public void Validate_Should_Not_Have_Error_When_StandardId_Is_Not_Empty()
        {
            // Arrange
            var model = new OverviewParameters { StandardId = "SomeStandardId" };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.StandardId);
        }

        [Test]
        public void Validate_Should_Not_Have_Error_When_RequestType_Is_Not_Empty()
        {
            // Arrange
            var model = new OverviewParameters { RequestType = RequestType.Providers };

            // Act
            var result = _sut.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.RequestType);
        }
    }
}

