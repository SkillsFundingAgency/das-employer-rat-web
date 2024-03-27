using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQueryValidator : AbstractValidator<GetEmployerRequestQuery>
    {
        public GetEmployerRequestQueryValidator()
        {
            RuleFor(x => x.EmployerRequestId).NotEmpty();
        }
    }
}
