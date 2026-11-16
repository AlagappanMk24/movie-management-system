using ReviewService.Domain.Enums;

namespace ReviewService.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing the payload required to update an existing movie review.
    /// </summary>
    public class UpdateReviewDto
    {
        /// <summary>Gets or sets the updated categorical classification of the review.</summary>
        public ReviewCategory Category { get; set; }

        /// <summary>Gets or sets the updated textual description or commentary for the review.</summary>
        public string? Description { get; set; }
    }
}