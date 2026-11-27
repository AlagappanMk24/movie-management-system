using System.Text.Json.Serialization;

namespace BookingService.Application.DTOs.Responses
{
    /// <summary>Represents external theatre details.</summary>
    public class TheatreDto
    {
        /// <summary>Gets or sets the unique theatre identifier.</summary>
        [JsonPropertyName("id")]
        public Guid TheatreId { get; set; }

        /// <summary>Gets or sets the commercial name of the theatre.</summary>
        public string TheatreName { get; set; } = string.Empty;

        /// <summary>Gets or sets the physical street address of the theatre.</summary>
        public string Address { get; set; } = string.Empty;
    }
}