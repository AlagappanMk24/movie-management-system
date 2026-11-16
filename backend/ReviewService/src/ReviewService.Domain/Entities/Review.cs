using ReviewService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReviewService.Domain.Entities
{
    /// <summary>
    /// Represents a user movie review entity stored in the database.
    /// </summary>
    public class Review
    {
        /// <summary>Gets or sets the unique primary key identifier for the review.</summary>
        [Key]
        public int ReviewId { get; set; }

        /// <summary>Gets or sets the cross-service movie identifier from the Movie service.</summary>
        [Required]
        public int MovieId { get; set; }

        /// <summary>Gets or sets the user identifier from the Authentication service.</summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>Gets or sets the categorical rating category of the review.</summary>
        [Required]
        public ReviewCategory Category { get; set; }

        /// <summary>Gets or sets the optional review comment description (maximum 1000 characters).</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>Gets or sets the timestamp when the review was originally created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the timestamp when the review was last updated, if modified.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}