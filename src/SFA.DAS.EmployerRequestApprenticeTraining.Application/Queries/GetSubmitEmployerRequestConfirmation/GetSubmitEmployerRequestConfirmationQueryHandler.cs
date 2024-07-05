using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetSubmitEmployerRequestConfirmation
{
    public class GetSubmitEmployerRequestConfirmationQueryHandler : IRequestHandler<GetSubmitEmployerRequestConfirmationQuery, SubmitEmployerRequestConfirmation>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetSubmitEmployerRequestConfirmationQuery> _validator;

        public GetSubmitEmployerRequestConfirmationQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetSubmitEmployerRequestConfirmationQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<SubmitEmployerRequestConfirmation> Handle(GetSubmitEmployerRequestConfirmationQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);
            var confirmation = await _outerApi.GetSubmitEmployerRequestConfirmation(request.EmployerRequestId);
            return confirmation;
        }
    }
}
