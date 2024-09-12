using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest
{
    public class GetExistingEmployerRequestQueryValidator : AbstractValidator<GetExistingEmployerRequestQuery>
    {
        public GetExistingEmployerRequestQueryValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty();
            
            RuleFor(x => x.StandardReference)
                .NotNull()
                .NotEmpty();
        }
    }
}
