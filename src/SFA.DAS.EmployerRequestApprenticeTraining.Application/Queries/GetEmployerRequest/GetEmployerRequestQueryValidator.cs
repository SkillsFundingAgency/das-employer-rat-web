using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQueryValidator : AbstractValidator<GetEmployerRequestQuery>
    {
        public GetEmployerRequestQueryValidator()
        {
            RuleFor(x => x.EmployerRequestId)
                .Must(x => x.HasValue)
                .When(x => string.IsNullOrEmpty(x.StandardReference) || !x.AccountId.HasValue);

            RuleFor(x => new { x.StandardReference, x.AccountId })
                .Must(x => !string.IsNullOrEmpty(x.StandardReference) && x.AccountId.HasValue)
                .When(x => !x.EmployerRequestId.HasValue);
        }
    }
}
