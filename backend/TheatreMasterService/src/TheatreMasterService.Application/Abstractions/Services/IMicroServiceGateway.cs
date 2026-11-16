namespace TheatreMasterService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines gateway contracts for querying booking data and seating inventory from downstream microservices.
    /// </summary>
    public interface IMicroServiceGateway
    {
        /// <summary>
        /// Checks whether a specific show has any active bookings associated with it.
        /// </summary>
        /// <param name="showId">The unique identifier of the show to check.</param>
        /// <returns>True if active bookings exist; otherwise, false.</returns>
        Task<bool> HasBookingByShow(int showId);

        /// <summary>
        /// Retrieves the total count of reserved seats for a given show from the booking service.
        /// </summary>
        /// <param name="showId">The unique identifier of the show.</param>
        /// <returns>The total number of booked seats.</returns>
        Task<int> GetBookedSeatsByShowAsync(int showId);
    }
}