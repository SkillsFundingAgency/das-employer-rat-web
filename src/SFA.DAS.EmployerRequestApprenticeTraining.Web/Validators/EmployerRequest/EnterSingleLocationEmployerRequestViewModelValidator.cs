using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterSingleLocationEmployerRequestViewModelValidator : AbstractValidator<EnterSingleLocationEmployerRequestViewModel>
    {
        public EnterSingleLocationEmployerRequestViewModelValidator(ILocationService locationService)
        {
            RuleFor(x => x.SingleLocation)
                .ValidateSingleLocation(locationService);
        }
    }

    public static class EnterSingleLocationEmployerRequestViewModelValidatorRules
    {
        public static IRuleBuilderOptions<T, string> ValidateSingleLocation<T>(this IRuleBuilder<T, string> ruleBuilder, ILocationService locationService)
        {
            return ruleBuilder
                .NotEmpty()
                    .WithMessage("Enter a town, city or postcode")
                .MustAsync((locationName, cancellationToken) => IsValidLocationAsync(locationName, locationService, cancellationToken))
                    .WithMessage("Enter a valid location");
        }

        private static async Task<bool> IsValidLocationAsync(string locationName, ILocationService locationService, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                return false;
            }

            return await locationService.CheckLocationExists(locationName);
        }
    }
}
