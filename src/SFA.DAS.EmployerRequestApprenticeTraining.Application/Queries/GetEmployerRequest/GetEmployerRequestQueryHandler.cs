using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQueryHandler : IRequestHandler<GetEmployerRequestQuery, EmployerRequest>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetEmployerRequestQuery> _validator;

        public GetEmployerRequestQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetEmployerRequestQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<EmployerRequest> Handle(GetEmployerRequestQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);
            var employerRequest = await _outerApi.GetEmployerRequest(request.EmployerRequestId);
            return employerRequest;
        }
    }
}
