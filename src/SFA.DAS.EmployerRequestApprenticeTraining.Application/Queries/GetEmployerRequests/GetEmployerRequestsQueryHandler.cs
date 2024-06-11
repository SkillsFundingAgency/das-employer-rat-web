using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests
{
    public class GetEmployerRequestsQueryHandler : IRequestHandler<GetEmployerRequestsQuery, List<EmployerRequest>>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetEmployerRequestsQuery> _validator;

        public GetEmployerRequestsQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetEmployerRequestsQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<List<EmployerRequest>> Handle(GetEmployerRequestsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);
            var employerRequests = await _outerApi.GetEmployerRequests(request.AccountId);
            return employerRequests;
        }
    }
}
