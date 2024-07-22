using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class CheckYourAnswersEmployerRequestViewModelValidator : AbstractValidator<CheckYourAnswersEmployerRequestViewModel>
    {
        public CheckYourAnswersEmployerRequestViewModelValidator(ILocationService locationService)
        {
            RuleFor(x => x.NumberOfApprentices)
                .ValidateNumberOfApprentices();

            RuleFor(x => x.SameLocation)
                .ValidateSameLocation()
                .When(x => int.TryParse(x.NumberOfApprentices, out int number) && number > 1);

            RuleFor(x => x.SingleLocation)
                .ValidateSingleLocation(locationService)
                .When(x => string.IsNullOrEmpty(x.SameLocation) || x.SameLocation == "Yes");

            RuleFor(x => x.MultipleLocations)
                .ValidateMultipleLocations()
                .When(x => x.SameLocation == "No");

            RuleFor(x => x.AtApprenticesWorkplace)
                .ValidateTrainingOptions();
        }
    }
}
