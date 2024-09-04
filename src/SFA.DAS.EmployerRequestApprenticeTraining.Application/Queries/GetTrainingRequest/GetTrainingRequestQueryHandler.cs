using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetTrainingRequest
{
    public class GetTrainingRequestQueryHandler : IRequestHandler<GetTrainingRequestQuery, TrainingRequest?>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly IValidator<GetTrainingRequestQuery> _validator;

        public GetTrainingRequestQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, IValidator<GetTrainingRequestQuery> validator)
        {
            _outerApi = outerApi;
            _validator = validator;
        }

        public async Task<TrainingRequest?> Handle(GetTrainingRequestQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);

            TrainingRequest trainingRequest = await _outerApi.GetTrainingRequest(request.EmployerRequestId);

            return trainingRequest;
        }
    }
}
