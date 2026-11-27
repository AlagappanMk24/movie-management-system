using BookingService.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BookingService.Infrastructure.Caching
{
    /// <inheritdoc />
    public class RedisCacheService(IDistributedCache cache) : ICacheService
    {
        private readonly IDistributedCache _cache = cache;

        /// <inheritdoc />
        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }

            var data = await factory();
            if (data != null)
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
            }

            return data;
        }

        /// <inheritdoc />
        public async Task<bool> TryLockSeatAsync(Guid showId, string seatNo, TimeSpan expiration)
        {
            string lockKey = $"lock_show_{showId}_seat_{seatNo}";
            if (await _cache.GetStringAsync(lockKey) != null)
            {
                return false;
            }

            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await _cache.SetStringAsync(lockKey, "locked", options);
            return true;
        }

        public async Task ReleaseSeatLockAsync(Guid showId, string seatNo)
            => await _cache.RemoveAsync($"lock_show_{showId}_seat_{seatNo}");
    }
}