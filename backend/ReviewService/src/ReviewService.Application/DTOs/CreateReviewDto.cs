using ReviewService.Domain.Enums;

namespace ReviewService.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing the payload required to create a new movie review.
    /// </summary>
    public class CreateReviewDto
    {
        /// <summary>Gets or sets the unique identifier of the movie being reviewed.</summary>
        public int MovieId { get; set; }

        /// <summary>Gets or sets the categorical classification of the review.</summary>
        public ReviewCategory Category { get; set; }

        /// <summary>Gets or sets the optional textual description or commentary for the review.</summary>
        public string? Description { get; set; }
    }
}