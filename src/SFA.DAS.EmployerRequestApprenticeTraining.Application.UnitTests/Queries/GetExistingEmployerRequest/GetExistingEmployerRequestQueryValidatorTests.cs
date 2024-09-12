using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetExistingEmployerRequest
{
    [TestFixture]
    public class GetExistingEmployerRequestQueryValidatorTests
    {
        private GetExistingEmployerRequestQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetExistingEmployerRequestQueryValidator();
        }

        [Test]
        public void Should_Have_Error_When_StandardReference_IsNull_Or_AccountId_IsNotSpecified()
        {
            var query = new GetExistingEmployerRequestQuery();
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.StandardReference);
            result.ShouldHaveValidationErrorFor(x => x.AccountId);
        }

        [Test]
        public void Should_Not_Have_Error_When_StandardReference_And_AccountId_Are_Present()
        {
            var query = new GetExistingEmployerRequestQuery { StandardReference = "SomeReference", AccountId = 12345 };
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.StandardReference);
            result.ShouldNotHaveValidationErrorFor(x => x.AccountId);
        }
    }
}
