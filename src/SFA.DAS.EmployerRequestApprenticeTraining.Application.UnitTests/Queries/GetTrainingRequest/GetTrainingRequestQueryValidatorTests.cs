using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetTrainingRequest
{
    [TestFixture]
    public class GetTrainingRequestQueryValidatorTests
    {
        private GetTrainingRequestQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetTrainingRequestQueryValidator();
        }

        [Test]
        public void Should_Have_Error_When_EmployerRequestId_IsEmpty()
        {
            var query = new GetTrainingRequestQuery();
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.EmployerRequestId);
        }

        [Test]
        public void Should_Not_Have_Error_When_EmployerRequestId_Is_Present()
        {
            var query = new GetTrainingRequestQuery { EmployerRequestId = Guid.NewGuid() };
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployerRequestId);
        }
    }
}
