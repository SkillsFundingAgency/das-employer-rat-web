using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest
{
    public class GetExistingEmployerRequestQueryHandler : IRequestHandler<GetExistingEmployerRequestQuery, bool>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetExistingEmployerRequestQuery> _validator;

        public GetExistingEmployerRequestQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetExistingEmployerRequestQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<bool> Handle(GetExistingEmployerRequestQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);

            var existing = await _outerApi.GetExistingEmployerRequest(request.AccountId, request.StandardReference);
            return existing;
        }
    }
}
