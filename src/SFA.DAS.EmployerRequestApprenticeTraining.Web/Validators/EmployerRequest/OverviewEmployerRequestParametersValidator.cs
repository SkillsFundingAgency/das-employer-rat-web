using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class OverviewEmployerRequestParametersValidator : AbstractValidator<OverviewEmployerRequestParameters>
    {
        public OverviewEmployerRequestParametersValidator()
        {
            RuleFor(x => x.StandardId)
                .NotEmpty()
                .WithMessage($"{ValidateRequiredQueryParametersAttribute.MissingRequireQueryParameterMessage}{nameof(OverviewEmployerRequestParameters.StandardId)}");

            RuleFor(x => x.RequestType)
                .NotEmpty()
                .WithMessage($"{ValidateRequiredQueryParametersAttribute.MissingRequireQueryParameterMessage}{nameof(OverviewEmployerRequestParameters.RequestType)}");
        }
    }
}
