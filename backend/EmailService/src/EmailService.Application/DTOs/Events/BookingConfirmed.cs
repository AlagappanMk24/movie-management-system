namespace EmailService.Application.DTOs.Events
{
    /// <summary>
    /// Represents the event contract published when a booking is confirmed.
    /// </summary>
    public class BookingConfirmed
    {
        /// <summary>Gets or sets the unique booking identifier.</summary>
        public int BookingId { get; set; }

        /// <summary>Gets or sets the show identifier.</summary>
        public int ShowId { get; set; }

        /// <summary>Gets or sets the total seat count.</summary>
        public int SeatCount { get; set; }

        /// <summary>Gets or sets the movie title.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the scheduled show time.</summary>
        public string ShowTime { get; set; } = string.Empty;

        /// <summary>Gets or sets the user identifier.</summary>
        public int UserId { get; set; }
    }
}