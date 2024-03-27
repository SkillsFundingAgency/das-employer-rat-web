using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class CreateEmployerRequestPostRequestValidator : AbstractValidator<CreateEmployerRequestPostRequest>
    {
        public CreateEmployerRequestPostRequestValidator()
        {
            RuleFor(x => x.RequestType)
                .NotEmpty()
                .WithMessage("You must choose a request type");
        }
    }
}
