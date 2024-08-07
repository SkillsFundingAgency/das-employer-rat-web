﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage
{
    public class CacheStorageService : ICacheStorageService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheStorageService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SaveToCache<T>(string key, T item, int expirationInHours)
        {
            var json = JsonConvert.SerializeObject(item);

            await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
            });
        }

        public async Task<T> RetrieveFromCache<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        public async Task DeleteFromCache(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
