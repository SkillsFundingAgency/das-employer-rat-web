using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterSingleLocationEmployerRequestViewModelValidator : AbstractValidator<EnterSingleLocationEmployerRequestViewModel>
    {
        private readonly ILocationService _locationService;

        public EnterSingleLocationEmployerRequestViewModelValidator(ILocationService locationService)
        {
            _locationService = locationService;

            RuleFor(x => x.SingleLocation)
                .NotEmpty().WithMessage("Enter a town, city or postcode")
                .DependentRules(() =>
                {
                    RuleFor(x => x.SingleLocation)
                        .MustAsync(IsValidLocationAsync).WithMessage("Enter a valid location");
                });
        }

        private async Task<bool> IsValidLocationAsync(string locationName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                return false;
            }

            return await _locationService.CheckLocationExists(locationName);
        }
    }
}
