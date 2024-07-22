using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetRegions
{
    public class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<Region>>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ICacheStorageService _cacheStorageService;

        public GetRegionsQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, ICacheStorageService cacheStorageService)
        {
            _outerApi = outerApi;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<List<Region>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            var regionsCacheKey = $"GetRegions";
            var regions = await _cacheStorageService.RetrieveFromCache<List<Region>>(regionsCacheKey);

            if (regions == null)
            {
                try
                {
                    regions = await _outerApi.GetRegions();
                    await _cacheStorageService.SaveToCache(regionsCacheKey, regions, 1);
                }
                catch (RestEase.ApiException ex)
                {
                    throw new InvalidOperationException($"The regions cannot be retrieved", ex);
                }
            }

            return regions;
        }
    }
}
