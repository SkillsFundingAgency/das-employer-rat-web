using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Queries.GetEmployerRequest
{
    [TestFixture]
    public class GetEmployerRequestQueryValidatorTests
    {
        private GetEmployerRequestQueryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GetEmployerRequestQueryValidator();
        }

        [Test]
        public void Should_Have_Error_When_EmployerRequestId_And_StandardReference_And_AccountId_Are_All_Null()
        {
            var query = new GetEmployerRequestQuery();
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.EmployerRequestId);
        }

        [Test]
        public void Should_Not_Have_Error_When_EmployerRequestId_Is_Present()
        {
            var query = new GetEmployerRequestQuery { EmployerRequestId = Guid.NewGuid() };
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployerRequestId);
        }

        [Test]
        public void Should_Have_Error_When_StandardReference_Is_Not_Present_And_AccountId_Is_Null()
        {
            var query = new GetEmployerRequestQuery { EmployerRequestId = null, StandardReference = "", AccountId = null };
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.EmployerRequestId);
        }

        [Test]
        public void Should_Not_Have_Error_When_StandardReference_And_AccountId_Are_Present()
        {
            var query = new GetEmployerRequestQuery { EmployerRequestId = null, StandardReference = "SomeReference", AccountId = 12345 };
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.StandardReference);
            result.ShouldNotHaveValidationErrorFor(x => x.AccountId);
        }
    }
}
