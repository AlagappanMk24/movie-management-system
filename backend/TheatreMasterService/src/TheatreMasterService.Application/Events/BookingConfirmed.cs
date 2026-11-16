namespace TheatreMasterService.Application.Events
{
    /// <summary>
    /// Represents the event contract payload consumed when a booking is successfully confirmed.
    /// </summary>
    public class BookingConfirmed
    {
        /// <summary>Gets or sets the unique booking identifier.</summary>
        public int BookingId { get; set; }

        /// <summary>Gets or sets the show identifier associated with the confirmed booking.</summary>
        public int ShowId { get; set; }

        /// <summary>Gets or sets the total seat count reserved in the booking.</summary>
        public int SeatCount { get; set; }
    }
}