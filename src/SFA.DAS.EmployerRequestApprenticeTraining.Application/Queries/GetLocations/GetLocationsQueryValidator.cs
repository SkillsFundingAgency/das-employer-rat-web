using FluentValidation;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetLocations
{
    public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
    {
        public GetLocationsQueryValidator()
        {
            RuleFor(x => x.SearchTerm).NotEmpty();
        }
    }
}
