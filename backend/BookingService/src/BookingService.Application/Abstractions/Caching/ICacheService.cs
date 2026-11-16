namespace BookingService.Application.Abstractions.Caching
{
    /// <summary>
    /// Defines distributed caching and seat-locking operations.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a cached item by key, or fetches and caches it using the provided factory.
        /// </summary>
        Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);

        /// <summary>
        /// Attempts to acquire a distributed lock for a specific show seat.
        /// </summary>
        Task<bool> TryLockSeatAsync(int showId, string seatNo, TimeSpan expiration);
    }
}