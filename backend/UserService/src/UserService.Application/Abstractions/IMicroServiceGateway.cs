namespace UserService.Application.Abstractions
{
    /// <summary>
    /// Defines communication contracts with external microservices via HTTP.
    /// </summary>
    public interface IMicroServiceGateway
    {
        /// <summary>
        /// Checks asynchronously whether a user has existing bookings.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if bookings exist; otherwise, false.</returns>
        Task<bool> HasBooking(Guid userId);
    }
}