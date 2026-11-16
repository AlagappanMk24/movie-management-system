using Microsoft.EntityFrameworkCore;
using ReviewService.Application.Abstractions.Persistence;
using ReviewService.Domain.Entities;
using ReviewService.Infrastructure.Persistence.Context;

namespace ReviewService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class ReviewRepository(ReviewDbContext context) : IReviewRepository
    {
        #region Configuration
        private readonly ReviewDbContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Review?> GetByMovieAndUserAsync(int movieId, int userId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }
    }
}