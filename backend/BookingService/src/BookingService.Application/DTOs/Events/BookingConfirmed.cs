namespace BookingService.Application.DTOs.Events
{
    /// <summary>
    /// Represents the event contract published when a booking is confirmed.
    /// </summary>
    public class BookingConfirmed
    {
        public int BookingId { get; set; }
        public int ShowId { get; set; }
        public int SeatCount { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan ShowTime { get; set; }
        public int UserId { get; set; }
    }
}
