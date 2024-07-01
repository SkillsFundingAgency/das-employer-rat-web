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

            RuleFor(x => x.SingleLocation)
                .ValidateSingleLocation(locationService);

            RuleFor(x => x.AtApprenticesWorkplace)
                .ValidateTrainingOptions();
        }
    }
}
