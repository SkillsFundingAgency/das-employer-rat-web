using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterApprenticesEmployerRequestViewModelValidator : AbstractValidator<EnterApprenticesEmployerRequestViewModel>
    {
        public EnterApprenticesEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.NumberOfApprentices)
                .ValidateNumberOfApprentices();
        }
    }

    public static class EnterApprenticesEmployerRequestViewModelValidatorRules
    {
        public static IRuleBuilderOptions<T, string> ValidateNumberOfApprentices<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                    .WithMessage("Enter the number of apprentices")
                .Must(BeValidNumber)
                    .WithMessage("Enter a number")
                .Must(BeWholeNumber)
                    .WithMessage("Enter a whole number")
                .Must(BeWithinValidRange)
                    .WithMessage("Enter a number between 1 and 9999");
        }

        private static bool BeValidNumber(string numberOfApprentices)
        {
            return int.TryParse(numberOfApprentices, out _);
        }

        private static bool BeWholeNumber(string numberOfApprentices)
        {
            return string.IsNullOrEmpty(numberOfApprentices) || !numberOfApprentices.Contains('.');
        }

        private static bool BeWithinValidRange(string numberOfApprentices)
        {
            if (int.TryParse(numberOfApprentices, out int number))
            {
                return number >= 1 && number <= 9999;
            }
            return false;
        }
    }
}
