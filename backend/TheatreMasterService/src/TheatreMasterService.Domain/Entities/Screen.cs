using System.Text.Json.Serialization;

namespace TheatreMasterService.Domain.Entities
{
    /// <summary>
    /// Represents a specific auditorium or screen inside a theatre.
    /// </summary>
    public class Screen
    {
        /// <summary>Gets or sets the unique identifier for the screen.</summary>
        public int ScreenId { get; set; }

        /// <summary>Gets or sets the foreign key identifier of the parent theatre.</summary>
        public int TheatreId { get; set; }

        /// <summary>Gets or sets the navigation property for the parent theatre.</summary>
        [JsonIgnore]
        public Theatre? Theatre { get; set; }

        /// <summary>Gets or sets the display name or number of the screen (e.g., Screen 1, IMAX).</summary>
        public string ScreenName { get; set; } = string.Empty;

        /// <summary>Gets or sets the projection/audio format type (e.g., 2D, 3D, IMAX, 4DX).</summary>
        public string ScreenType { get; set; } = string.Empty;

        /// <summary>Gets or sets the maximum seating capacity of the screen.</summary>
        public int SeatCapacity { get; set; }

        /// <summary>Gets or sets the collection of movie shows scheduled for this screen.</summary>
        [JsonIgnore]
        public ICollection<Show>? Shows { get; set; }

        /// <summary>Gets or sets the timestamp when the screen record was created.</summary>
        public DateTime Created { get; set; }

        /// <summary>Gets or sets the timestamp when the screen record was last modified.</summary>
        public DateTime Modified { get; set; }
    }
}