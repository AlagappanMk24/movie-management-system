using System.Text.Json.Serialization;

namespace TheatreMasterService.Domain.Entities
{
    /// <summary>
    /// Represents a cinema theatre complex hosting multiple screens and shows.
    /// </summary>
    public class Theatre
    {
        /// <summary>Gets or sets the unique identifier for the theatre.</summary>
        public int TheatreId { get; set; }

        /// <summary>Gets or sets the commercial name of the theatre complex.</summary>
        public string TheatreName { get; set; } = string.Empty;

        /// <summary>Gets or sets the city where the theatre is located.</summary>
        public string City { get; set; } = string.Empty;

        /// <summary>Gets or sets the physical street address of the theatre.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the contact telephone number for the theatre.</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the timestamp when the theatre record was created.</summary>
        public DateTime Created { get; set; }

        /// <summary>Gets or sets the timestamp when the theatre record was last modified.</summary>
        public DateTime Modified { get; set; }

        /// <summary>Gets or sets the collection of screens available in this theatre complex.</summary>
        [JsonIgnore]
        public ICollection<Screen>? Screens { get; set; }
    }
}