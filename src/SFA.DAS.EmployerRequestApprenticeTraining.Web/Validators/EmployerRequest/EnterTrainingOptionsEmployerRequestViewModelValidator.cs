using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Validators
{
    public class EnterTrainingOptionsEmployerRequestViewModelValidator : AbstractValidator<EnterTrainingOptionsEmployerRequestViewModel>
    {
        public EnterTrainingOptionsEmployerRequestViewModelValidator()
        {
            RuleFor(x => x.AtApprenticesWorkplace)
                .Must((model, value) => AtLeastOneOptionSelected(model))
                .WithMessage("Select a training option");
        }

        private bool AtLeastOneOptionSelected(EnterTrainingOptionsEmployerRequestViewModel model)
        {
            return model.AtApprenticesWorkplace || model.DayRelease || model.BlockRelease;
        }
    }
}
