using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;
using System.Net;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetClosestRegion
{
    public class GetClosestRegionQueryHandler : IRequestHandler<GetClosestRegionQuery, Region?>
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ICacheStorageService _cacheStorageService;

        public GetClosestRegionQueryHandler(IEmployerRequestApprenticeTrainingOuterApi outerApi, ICacheStorageService cacheStorageService)
        {
            _outerApi = outerApi;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<Region?> Handle(GetClosestRegionQuery request, CancellationToken cancellationToken)
        {
            var closestRegionCacheKey = $"GetClosestRegion:{request.Location}";
            var region = await _cacheStorageService.RetrieveFromCache<Region>(closestRegionCacheKey);

            if (region == null)
            {
                try
                {
                    region = await _outerApi.GetClosestRegion(request.Location);
                    await _cacheStorageService.SaveToCache(closestRegionCacheKey, region, 1);
                }
                catch (RestEase.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                catch (RestEase.ApiException ex) 
                {
                    throw new InvalidOperationException($"The closest region for {request.Location} cannot be retrieved", ex);
                }
            }

            return region;
        }
    }
}
