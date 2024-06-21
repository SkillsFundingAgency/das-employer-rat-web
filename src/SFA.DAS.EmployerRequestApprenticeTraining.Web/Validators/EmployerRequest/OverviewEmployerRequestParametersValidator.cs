using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class OverviewEmployerRequestParametersValidator : AbstractValidator<SubmitEmployerRequestParameters>
    {
        public OverviewEmployerRequestParametersValidator()
        {
            RuleFor(x => x.StandardId)
                .NotEmpty()
                .WithMessage($"{ValidateRequiredQueryParametersAttribute.MissingRequireQueryParameterMessage}{nameof(SubmitEmployerRequestParameters.StandardId)}");

            RuleFor(x => x.RequestType)
                .Must(NotInvalidRequestType)
                .WithMessage($"{ValidateRequiredQueryParametersAttribute.MissingRequireQueryParameterMessage}{nameof(SubmitEmployerRequestParameters.RequestType)}");
        }

        private bool NotInvalidRequestType(RequestType requestType)
        {
            return Enum.IsDefined(typeof(RequestType), requestType);
        }
    }
}
