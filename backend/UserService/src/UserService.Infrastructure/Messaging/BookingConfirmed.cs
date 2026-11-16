namespace UserService.Infrastructure.Messaging
{
    /// <summary>
    /// Represents the message contract for a confirmed booking event.
    /// </summary>
    public class BookingConfirmed
    {
        /// <summary>Gets or sets the booking identifier.</summary>
        public int BookingId { get; set; }

        /// <summary>Gets or sets the show identifier.</summary>
        public int ShowId { get; set; }

        /// <summary>Gets or sets the number of seats booked.</summary>
        public int SeatCount { get; set; }

        /// <summary>Gets or sets the movie title.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the show time.</summary>
        public DateTime ShowTime { get; set; }
    }
}