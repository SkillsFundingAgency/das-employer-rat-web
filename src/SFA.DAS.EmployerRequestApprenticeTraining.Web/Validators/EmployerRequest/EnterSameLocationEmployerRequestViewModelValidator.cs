using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterSameLocationEmployerRequestViewModelValidator : AbstractValidator<EnterSameLocationEmployerRequestViewModel>
    {
        public EnterSameLocationEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.SameLocation)
                .ValidateSameLocation();
        }
    }

    public static class EnterSameLocationEmployerRequestViewModelValidatorRules
    {
        public static IRuleBuilderOptions<T, string> ValidateSameLocation<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                    .WithMessage("You must make a selection");
        }
    }
}
