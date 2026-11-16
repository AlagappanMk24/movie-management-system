namespace BookingService.Application.DTOs.Responses
{
    /// <summary>Represents external theatre details.</summary>
    public class TheatreDto
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}