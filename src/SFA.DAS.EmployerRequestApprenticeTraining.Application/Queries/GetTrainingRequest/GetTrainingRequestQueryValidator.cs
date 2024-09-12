using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest
{
    public class GetTrainingRequestQueryValidator : AbstractValidator<GetTrainingRequestQuery>
    {
        public GetTrainingRequestQueryValidator()
        {
            RuleFor(x => x.EmployerRequestId)
                .NotEmpty();
        }
    }
}
