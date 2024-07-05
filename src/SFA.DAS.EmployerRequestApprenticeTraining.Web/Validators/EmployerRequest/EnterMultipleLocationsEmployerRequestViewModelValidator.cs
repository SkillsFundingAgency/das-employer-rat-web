using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterMultipleLocationsEmployerRequestViewModelValidator : AbstractValidator<EnterMultipleLocationsEmployerRequestViewModel>
    {
        public EnterMultipleLocationsEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.SelectedSubRegions)
                .ValidateMultipleLocations();
        }
    }

    public static class EnterMultipleLocationsEmployerRequestViewModelValidatorRules
    {
        public static IRuleBuilderOptions<T, string[]> ValidateMultipleLocations<T>(this IRuleBuilder<T, string[]> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                    .WithMessage("Select a location")
                .Must(subRegions => subRegions != null && subRegions.Length > 0)
                    .WithMessage("Select a location");
        }
    }
}