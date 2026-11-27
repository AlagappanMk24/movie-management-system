using ReviewService.Application.Abstractions.Persistence;
using ReviewService.Application.Abstractions.Services;
using ReviewService.Application.DTOs;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Enums;

namespace ReviewService.Application.Services
{
    /// <inheritdoc />
    public class ReviewService(IReviewRepository repository) : IReviewService
    {
        #region configuration
        private readonly IReviewRepository _repository = repository;
        #endregion

        /// <inheritdoc />
        public async Task CreateReviewAsync(CreateReviewDto dto, Guid userId)
        {
            if (!Enum.IsDefined(typeof(ReviewCategory), dto.Category))
                throw new Exception("Invalid review category");

            var existingReview =
                await _repository.GetByMovieAndUserAsync(dto.MovieId, userId);

            if (existingReview != null)
                throw new Exception("You already reviewed this movie");

            var review = new Review
            {
                MovieId = dto.MovieId,
                UserId = userId,
                Category = dto.Category,
                Description = dto.Description,
                Created = DateTime.UtcNow
            };

            await _repository.AddAsync(review);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetReviewsByMovieAsync(Guid movieId)
        {
            var reviews = await _repository.GetByMovieIdAsync(movieId);

            return reviews.Select(r => new Review
            {
                Id = r.Id,
                MovieId = r.MovieId,
                UserId = r.UserId,
                Category = r.Category,
                Description = r.Description,
                Created = r.Created
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var reviews = await _repository.GetAllAsync();

            return reviews.Select(r => new Review
            {
                Id = r.Id,
                MovieId = r.MovieId,
                UserId = r.UserId,
                Category = r.Category,
                Description = r.Description,
                Created = r.Created
            });
        }

        /// <inheritdoc />
        public async Task UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, Guid userId)
        {
            if (!Enum.IsDefined(typeof(ReviewCategory), dto.Category))
                throw new Exception("Invalid review category");

            var review = await _repository.GetByIdAsync(reviewId);

            if (review == null)
                throw new Exception("Review not found");

            if (review.UserId != userId)
                throw new Exception("You are not allowed to update this review");

            review.Category = dto.Category;
            review.Description = dto.Description;
            review.Modified = DateTime.UtcNow;

            await _repository.UpdateAsync(review);
        }
    }
}