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
        private const int minNumberOfApprentices = 1;
        private const int maxNumberOfApprentices = 9999;
        
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
                    .WithMessage($"Enter a number between {minNumberOfApprentices} and {maxNumberOfApprentices}");
        }

        private static bool BeValidNumber(string numberOfApprentices)
        {
            return decimal.TryParse(numberOfApprentices, out _);
        }

        private static bool BeWholeNumber(string numberOfApprentices)
        {
            return int.TryParse(numberOfApprentices, out _);
        }

        private static bool BeWithinValidRange(string numberOfApprentices)
        {
            if (int.TryParse(numberOfApprentices, out int number))
            {
                return number >= minNumberOfApprentices && number <= maxNumberOfApprentices;
            }

            return false;
        }
    }
}
