using ReviewService.Application.DTOs;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for managing movie reviews.
    /// </summary>
    public interface IReviewService
    {
        /// <summary>Creates a new review for a movie.</summary>
        /// <param name="dto">The creation payload containing review details.</param>
        /// <param name="userId">The identifier of the user creating the review.</param>
        Task CreateReviewAsync(CreateReviewDto dto, int userId);

        /// <summary>Retrieves all reviews for a specific movie.</summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of reviews.</returns>
        Task<IEnumerable<Review>> GetReviewsByMovieAsync(int movieId);

        /// <summary>Retrieves all system reviews.</summary>
        /// <returns>A collection of all reviews.</returns>
        Task<IEnumerable<Review>> GetAllReviewsAsync();

        /// <summary>Updates an existing review.</summary>
        /// <param name="reviewId">The review identifier.</param>
        /// <param name="dto">The update payload.</param>
        /// <param name="userId">The identifier of the user performing the update.</param>
        Task UpdateReviewAsync(int reviewId, UpdateReviewDto dto, int userId);
    }
}