using ReviewService.Domain.Enums;
using Shared.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace ReviewService.Domain.Entities
{
    /// <summary>
    /// Represents a user movie review entity stored in the database.
    /// </summary>
    public class Review : BaseEntity
    {
        /// <summary>Gets or sets the cross-service movie identifier from the Movie service.</summary>
        [Required]
        public Guid MovieId { get; set; }

        /// <summary>Gets or sets the user identifier from the Authentication service.</summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>Gets or sets the categorical rating category of the review.</summary>
        [Required]
        public ReviewCategory Category { get; set; }

        /// <summary>Gets or sets the optional review comment description (maximum 1000 characters).</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}