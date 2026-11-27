namespace BookingService.Application.DTOs.Requests
{
    /// <summary>
    /// Represents a request payload for bulk locking and booking seats for an existing booking.
    /// </summary>
    public class BulkSeatBookingRequest
    {
        /// <summary>Gets or sets the booking identifier.</summary>
        public Guid BookingId { get; set; }

        /// <summary>Gets or sets the show identifier.</summary>
        public Guid ShowId { get; set; }

        /// <summary>Gets or sets the list of seat numbers to reserve.</summary>
        public List<string> SeatNos { get; set; } = [];
    }
}