namespace EmailService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines gateway contracts for fetching user email addresses and downloading PDF tickets from downstream services.
    /// </summary>
    public interface IMicroServiceGateway
    {
        /// <summary>Downloads the generated PDF ticket binary from the booking service.</summary>
        Task<byte[]> DownloadTicketAsync(Guid bookingId);

        /// <summary>Retrieves the user's email address by their unique identifier.</summary>
        Task<string> GetUserEmailAsync(Guid userId);
    }
}