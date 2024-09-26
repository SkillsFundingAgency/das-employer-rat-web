﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.UnitTests.Services.CacheStorage
{
    [TestFixture]
    public class CacheStorageServiceTests
    {
        private Mock<IDistributedCache> _distributedCacheMock;
        private CacheStorageService _cacheStorageService;

        [SetUp]
        public void Setup()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _cacheStorageService = new CacheStorageService(_distributedCacheMock.Object);
        }

        [Test]
        public async Task SaveToCache_ShouldStoreItemInCacheWithExpiration()
        {
            // Arrange
            var key = "testKey";
            var item = new Standard
            {
                StandardReference = "IfateRef123",
                StandardTitle = "Test Title",
                StandardLevel = 3
            };

            var expirationInHours = 1;
            var json = JsonConvert.SerializeObject(item);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
            };

            // Act
            await _cacheStorageService.SaveToCache(key, item, expirationInHours);

            // Assert
            _distributedCacheMock.Verify(c => c.SetAsync(key,
                It.Is<byte[]>(v => System.Text.Encoding.UTF8.GetString(v) == json),
                It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == options.AbsoluteExpirationRelativeToNow),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RetrieveFromCache_ShouldReturnItemFromCache()
        {
            // Arrange
            var key = "testKey";
            var expectedItem = new Standard
            {
                StandardReference = "IfateRef123",
                StandardTitle = "Test Title",
                StandardLevel = 3
            };

            var json = JsonConvert.SerializeObject(expectedItem);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(json);

            _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(byteArray);

            // Act
            var result = await _cacheStorageService.RetrieveFromCache<Standard>(key);

            // Assert
            result.Should().BeEquivalentTo(expectedItem);
        }

        [Test]
        public async Task RetrieveFromCache_ShouldReturnDefaultWhenItemNotInCache()
        {
            // Arrange
            var key = "testKey";

            _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync((byte[])null);

            // Act
            var result = await _cacheStorageService.RetrieveFromCache<Standard>(key);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task DeleteFromCache_ShouldRemoveItemFromCache()
        {
            // Arrange
            var key = "testKey";

            // Act
            await _cacheStorageService.DeleteFromCache(key);

            // Assert
            _distributedCacheMock.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}