using ReviewService.Domain.Entities;

namespace ReviewService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository contract operations for managing movie review data persistence.
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>Retrieves a specific review by movie and user identifier.</summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The matching review object, or null if not found.</returns>
        Task<Review?> GetByMovieAndUserAsync(Guid movieId, Guid userId);

        /// <summary>Retrieves all reviews associated with a specific movie identifier.</summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of reviews for the movie.</returns>
        Task<IEnumerable<Review>> GetByMovieIdAsync(Guid movieId);

        /// <summary>Adds a new review record.</summary>
        /// <param name="review">The review entity to add.</param>
        Task AddAsync(Review review);

        /// <summary>Updates an existing review record.</summary>
        /// <param name="review">The review entity to update.</param>
        Task UpdateAsync(Review review);

        /// <summary>Retrieves all review records.</summary>
        /// <returns>A collection of all reviews.</returns>
        Task<IEnumerable<Review>> GetAllAsync();

        /// <summary>Retrieves a specific review by its unique identifier.</summary>
        /// <param name="reviewId">The review identifier.</param>
        /// <returns>The matching review object, or null if not found.</returns>
        Task<Review?> GetByIdAsync(Guid reviewId);
    }
}