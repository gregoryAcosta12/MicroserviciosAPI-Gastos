using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Kernel.Caching;
using Xunit;

namespace Shared.Kernel.Tests.IntegrationTests.Caching
{
    public class RedisIntegrationTests : IAsyncLifetime
    {
        private RedisCacheService? _cacheService;
        private readonly Mock<ILogger<RedisCacheService>> _loggerMock = new();

        public async Task InitializeAsync()
        {
            // Usar una instancia de Redis en memoria o real
            var connectionString = "localhost:6379";
            _cacheService = new RedisCacheService(connectionString, _loggerMock.Object);
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _cacheService?.Dispose();
            await Task.CompletedTask;
        }

        [Fact(Skip = "Requiere Redis corriendo")]
        public async Task SetAndGet_ShouldStoreAndRetrieveValue()
        {
            // Arrange
            var key = "test_key";
            var expectedValue = new TestData { Id = 1, Name = "Test" };

            // Act
            await _cacheService!.SetAsync(key, expectedValue, TimeSpan.FromMinutes(1));
            var actualValue = await _cacheService.GetAsync<TestData>(key);

            // Assert
            actualValue.Should().BeEquivalentTo(expectedValue);
        }

        [Fact(Skip = "Requiere Redis corriendo")]
        public async Task Remove_ShouldDeleteValue()
        {
            // Arrange
            var key = "test_key_to_remove";
            await _cacheService!.SetAsync(key, "Test value");

            // Act
            await _cacheService.RemoveAsync(key);
            var exists = await _cacheService.ExistsAsync(key);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact(Skip = "Requiere Redis corriendo")]
        public async Task Exists_ShouldReturnTrueForExistingKey()
        {
            // Arrange
            var key = "test_key_exists";
            await _cacheService!.SetAsync(key, "Test value");

            // Act
            var exists = await _cacheService.ExistsAsync(key);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact(Skip = "Requiere Redis corriendo")]
        public async Task RemoveByPattern_ShouldDeleteMatchingKeys()
        {
            // Arrange
            await _cacheService!.SetAsync("test_1", "value1");
            await _cacheService.SetAsync("test_2", "value2");
            await _cacheService.SetAsync("other_1", "value3");

            // Act
            await _cacheService.RemoveByPatternAsync("test");

            // Assert
            (await _cacheService.ExistsAsync("test_1")).Should().BeFalse();
            (await _cacheService.ExistsAsync("test_2")).Should().BeFalse();
            (await _cacheService.ExistsAsync("other_1")).Should().BeTrue();
        }
    }

    public class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}