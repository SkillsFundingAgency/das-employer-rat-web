using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterApprenticesEmployerRequestViewModelValidator : AbstractValidator<EnterApprenticesEmployerRequestViewModel>
    {
        public EnterApprenticesEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.NumberOfApprentices)
                .NotEmpty()
                .WithMessage("Enter the number of apprentices")
                .DependentRules(() =>
                {
                    RuleFor(x => x.NumberOfApprentices)
                        .Must(BeValidNumber)
                        .WithMessage("Enter a number")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.NumberOfApprentices)
                                .Must(BeWithinValidRange)
                                .WithMessage("Enter a number between 1 and 9999");
                        });
                });
        }

        private bool BeValidNumber(string numberOfApprentices)
        {
            return int.TryParse(numberOfApprentices, out _);
        }

        private bool BeWithinValidRange(string numberOfApprentices)
        {
            if (int.TryParse(numberOfApprentices, out int number))
            {
                return number >= 1 && number <= 9999;
            }
            return false;
        }
    }
}
