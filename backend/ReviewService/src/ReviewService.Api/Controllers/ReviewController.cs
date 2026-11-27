using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.Abstractions.Services;
using ReviewService.Application.DTOs;
using ReviewService.Domain.Entities;

namespace ReviewService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for creating, retrieving, and updating movie reviews and ratings.
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    public class ReviewController(IReviewService reviewService, ILogger<ReviewController> logger) : ControllerBase
    {
        #region Configuration
        private readonly IReviewService _reviewService = reviewService;
        private readonly ILogger<ReviewController> _logger = logger;
        #endregion

        /// <summary>
        /// Creates a new movie review.
        /// </summary>
        /// <param name="dto">The review creation payload.</param>
        /// <param name="userId">The identifier of the user submitting the review.</param>
        /// <returns>A success message indicating the review was added.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto, [FromQuery] Guid userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
                }

                await _reviewService.CreateReviewAsync(dto, userId);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Review added successfully."
                });
            }
           catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a review for Movie ID {MovieId} by User ID {UserId}.", dto?.MovieId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while creating the review.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves all reviews associated with a specific movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of reviews for the movie.</returns>
        [HttpGet("movie/{movieId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviews(Guid movieId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByMovieAsync(movieId);
                return Ok(new
                {
                    success = true,
                    message = "Movie reviews retrieved successfully.",
                    data = reviews
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reviews for Movie ID {MovieId}.", movieId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving movie reviews.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing movie review.
        /// </summary>
        /// <param name="reviewId">The unique review identifier.</param>
        /// <param name="dto">The review update payload.</param>
        /// <param name="userId">The identifier of the user performing the update.</param>
        /// <returns>A success message indicating the review was updated.</returns>
        [HttpPut("{reviewId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDto dto, [FromQuery] Guid userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
                }

                await _reviewService.UpdateReviewAsync(reviewId, dto, userId);
                return Ok(new
                {
                    success = true,
                    message = "Review updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Review with ID {ReviewId} was not found for update.", reviewId);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "User ID {UserId} is not authorized to update Review ID {ReviewId}.", userId, reviewId);
                return StatusCode(StatusCodes.Status401Unauthorized, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Review ID {ReviewId}.", reviewId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the review.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves all reviews across all movies in the system.
        /// </summary>
        /// <returns>A collection of all review records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(new
                {
                    success = true,
                    message = "All reviews retrieved successfully.",
                    data = reviews
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all reviews.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving reviews.",
                    details = ex.Message
                });
            }
        }
    }
}