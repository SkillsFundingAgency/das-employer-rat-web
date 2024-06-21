using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterTrainingOptionsEmployerRequestViewModelValidator : AbstractValidator<EnterTrainingOptionsEmployerRequestViewModel>
    {
        public EnterTrainingOptionsEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.AtApprenticesWorkplace)
                .ValidateTrainingOptions();
        }
    }

    public static class EnterTrainingOptionsEmployerRequestViewModelValidatorRules
    {
        public static IRuleBuilderOptionsConditions<T, bool> ValidateTrainingOptions<T>(this IRuleBuilder<T, bool> ruleBuilder)
            where T : IEnterTrainingOptionsEmployerRequestViewModel
        {
            return ruleBuilder.Custom((value, context) =>
            {
                var model = context.InstanceToValidate;
                if (!model.AtApprenticesWorkplace && !model.DayRelease && !model.BlockRelease)
                {
                    context.AddFailure("AtApprenticesWorkplace", "Select a training option");
                }
            });
        }
    }
}
