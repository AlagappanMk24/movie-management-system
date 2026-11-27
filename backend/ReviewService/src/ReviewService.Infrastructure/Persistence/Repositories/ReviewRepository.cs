using ReviewService.Application.Abstractions.Persistence;
using ReviewService.Domain.Entities;
using ReviewService.Infrastructure.Persistence.Context;
using System.Data;
using Dapper;

namespace ReviewService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class ReviewRepository(DapperContext context) : IReviewRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Review>(
                "SP_GetReviews",
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Review?> GetByIdAsync(Guid reviewId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Review>(
                "SP_GetReviewById",
                new { Id = reviewId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Review?> GetByMovieAndUserAsync(Guid movieId, Guid userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Review>(
                "SP_GetReviewByMovieAndUser",
                new { MovieId = movieId, UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Review>> GetByMovieIdAsync(Guid movieId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Review>(
                "SP_GetReviewsByMovieId",
                new { MovieId = movieId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task AddAsync(Review review)
        {
            using var connection = _context.CreateConnection();

            review.Id = review.Id == Guid.Empty ? Guid.NewGuid() : review.Id;
            review.Created = DateTime.UtcNow;
            review.Modified = DateTime.UtcNow;
            review.IsDeleted = false;

            await connection.QuerySingleAsync<Review>(
                "SP_CreateReview",
                new
                {
                    review.Id,
                    review.MovieId,
                    review.UserId,
                    Category = (int)review.Category,
                    review.Description,
                    review.Created,
                    review.Modified,
                    review.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Review review)
        {
            using var connection = _context.CreateConnection();

            review.Modified = DateTime.UtcNow;

            await connection.ExecuteScalarAsync<int>(
                "SP_UpdateReview",
                new
                {
                    Id = review.Id,
                    Category = (int)review.Category,
                    review.Description,
                    review.Modified,
                    review.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}