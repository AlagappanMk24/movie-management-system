namespace BookingService.Application.DTOs.Requests
{
    /// <summary>
    /// Represents a request payload for creating a new booking with associated seat selections transactionally.
    /// </summary>
    public class CreateBookingWithSeatsRequest
    {
        /// <summary>Gets or sets the show identifier.</summary>
        public Guid ShowId { get; set; }

        /// <summary>Gets or sets the user identifier making the booking.</summary>
        public Guid UserId { get; set; }

        /// <summary>Gets or sets the list of seat numbers to reserve.</summary>
        public List<string> SeatNos { get; set; } = [];
    }
}