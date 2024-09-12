using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetCancelEmployerRequestConfirmation

{
    public class GetCancelEmployerRequestConfirmationQueryValidator : AbstractValidator<GetCancelEmployerRequestConfirmationQuery>
    {
        public GetCancelEmployerRequestConfirmationQueryValidator()
        {
            RuleFor(x => x.EmployerRequestId).NotEmpty();
        }
    }
}
