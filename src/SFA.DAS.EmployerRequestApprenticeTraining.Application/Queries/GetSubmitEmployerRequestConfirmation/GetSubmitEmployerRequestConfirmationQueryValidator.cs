using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation

{
    public class GetSubmitEmployerRequestConfirmationQueryValidator : AbstractValidator<GetSubmitEmployerRequestConfirmationQuery>
    {
        public GetSubmitEmployerRequestConfirmationQueryValidator()
        {
            RuleFor(x => x.EmployerRequestId).NotEmpty();
        }
    }
}
