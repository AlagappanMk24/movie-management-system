using Shared.Contracts.Common;
using System.Text.Json.Serialization;

namespace TheatreMasterService.Domain.Entities
{
    /// <summary>
    /// Represents a scheduled movie screening instance on a specific screen and time.
    /// </summary>
    public class Show : BaseEntity
    {
        /// <summary>Gets or sets the cross-service movie identifier (managed by the Movie microservice).</summary>
        public Guid MovieId { get; set; }

        /// <summary>Gets or sets the foreign key identifier for the screening screen.</summary>
        public Guid ScreenId { get; set; }

        /// <summary>Gets or sets the navigation property for the screening screen.</summary>
        [JsonIgnore]
        public Screen? Screen { get; set; }

        /// <summary>Gets or sets the calendar date of the movie show.</summary>
        public DateTime ShowDate { get; set; }

        /// <summary>Gets or sets the scheduled start time of the show.</summary>
        public TimeSpan ShowTime { get; set; }

        /// <summary>Gets or sets the ticket price for this show.</summary>
        public int Price { get; set; }
    }
}