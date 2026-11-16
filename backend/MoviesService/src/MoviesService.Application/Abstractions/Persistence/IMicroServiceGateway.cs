namespace MoviesService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Gateway interface for communicating with external microservices to check cross-service dependencies.
    /// </summary>
    public interface IMicroServiceGateway
    {
        /// <summary>
        /// Checks whether the specified movie has any active shows registered in the theatre service.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>True if active shows exist; otherwise, false.</returns>
        Task<bool> HasShow(int movieId);

        /// <summary>
        /// Checks whether the specified movie has any active bookings in the booking service.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>True if active bookings exist; otherwise, false.</returns>
        Task<bool> HasBooking(int movieId);
    }
}