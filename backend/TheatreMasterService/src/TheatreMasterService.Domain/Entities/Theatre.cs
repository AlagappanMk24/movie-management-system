using Shared.Contracts.Common;
using System.Text.Json.Serialization;

namespace TheatreMasterService.Domain.Entities
{
    /// <summary>
    /// Represents a cinema theatre complex hosting multiple screens and shows.
    /// </summary>
    public class Theatre : BaseEntity
    {
        /// <summary>Gets or sets the commercial name of the theatre complex.</summary>
        public string TheatreName { get; set; } = string.Empty;

        /// <summary>Gets or sets the city where the theatre is located.</summary>
        public string City { get; set; } = string.Empty;

        /// <summary>Gets or sets the physical street address of the theatre.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the contact telephone number for the theatre.</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the collection of screens available in this theatre complex.</summary>
        [JsonIgnore]
        public ICollection<Screen>? Screens { get; set; }
    }
}