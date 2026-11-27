using Shared.Contracts.Common;
using System.Text.Json.Serialization;

namespace TheatreMasterService.Domain.Entities
{
    /// <summary>
    /// Represents a specific auditorium or screen inside a theatre.
    /// </summary>
    public class Screen : BaseEntity
    {
        /// <summary>Gets or sets the foreign key identifier of the parent theatre.</summary>
        public Guid TheatreId { get; set; }

        /// <summary>Gets or sets the navigation property for the parent theatre.</summary>
        [JsonIgnore]
        public Theatre? Theatre { get; set; }

        /// <summary>Gets or sets the display name or number of the screen (e.g., Screen 1, IMAX).</summary>
        public string ScreenName { get; set; } = string.Empty;

        /// <summary>Gets or sets the projection/audio format type (e.g., 2D, 3D, IMAX, 4DX).</summary>
        public string ScreenType { get; set; } = string.Empty;

        /// <summary>Gets or sets the maximum seating capacity of the screen.</summary>
        public int SeatCapacity { get; set; }

        // NEW
        public int Rows { get; set; }              // e.g., 10 → rows A..J
        public int SeatsPerRow { get; set; }       // e.g., 12 → A1..A12

        // Derived
        public int ComputedCapacity => Rows * SeatsPerRow;

        /// <summary>Gets or sets the collection of movie shows scheduled for this screen.</summary>
        [JsonIgnore]
        public ICollection<Show>? Shows { get; set; }
    }
}