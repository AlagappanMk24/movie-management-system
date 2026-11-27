using System.Text.Json.Serialization;

namespace BookingService.Application.DTOs.Responses
{
    /// <summary>Represents external screen details.</summary>
    public class ScreenDto
    {
        [JsonPropertyName("id")]
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public Guid TheatreId { get; set; }
    }
}