using FluentValidation;
using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetStandardQueryHandler : IRequestHandler<GetStandardQuery, StandardResponse>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IValidator<GetStandardQuery> _validator;

        public GetStandardQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, ICacheStorageService cacheStorageService, IValidator<GetStandardQuery> validator)
        {
            _outerApi = outerApi;
            _cacheStorageService = cacheStorageService;
            _validator = validator;
        }

        public async Task<StandardResponse> Handle(GetStandardQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request, cancellationToken);

            var standardCacheKey = $"GetStandard:{request.StandardId}";
            var standard = await _cacheStorageService.RetrieveFromCache<StandardResponse?>(standardCacheKey);
            
            if (standard == null)
            {
                try
                {
                    standard = await _outerApi.GetStandard(request.StandardId);
                    await _cacheStorageService.SaveToCache(standardCacheKey, standard, 1);
                }
                catch(RestEase.ApiException ex)
                {
                    throw new InvalidOperationException($"The standard {request.StandardId} cannot be found", ex);
                }
            }

            return standard;
        }
    }
}
