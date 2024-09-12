using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetCancelEmployerRequestConfirmation
{
    public class GetCancelEmployerRequestConfirmationQueryHandler : IRequestHandler<GetCancelEmployerRequestConfirmationQuery, CancelEmployerRequestConfirmation>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetCancelEmployerRequestConfirmationQuery> _validator;

        public GetCancelEmployerRequestConfirmationQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetCancelEmployerRequestConfirmationQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<CancelEmployerRequestConfirmation> Handle(GetCancelEmployerRequestConfirmationQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);
            var confirmation = await _outerApi.GetCancelEmployerRequestConfirmation(request.EmployerRequestId);
            return confirmation;
        }
    }
}
