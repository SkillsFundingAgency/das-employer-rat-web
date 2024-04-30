using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests
{
    public class GetEmployerRequestsQueryValidator : AbstractValidator<GetEmployerRequestsQuery>
    {
        public GetEmployerRequestsQueryValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}
