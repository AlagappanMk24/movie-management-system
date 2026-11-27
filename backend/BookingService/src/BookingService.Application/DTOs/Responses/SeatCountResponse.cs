namespace BookingService.Application.DTOs.Responses
{
    public class SeatCountResponse
    {
        public Guid ShowId { get; set; }
        public int BookedSeatCount { get; set; }
    }
}