using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetStandardQueryValidator : AbstractValidator<GetStandardQuery>
    {
        public GetStandardQueryValidator()
        {
            RuleFor(x => x.StandardId).NotEmpty();
        }
    }
}
