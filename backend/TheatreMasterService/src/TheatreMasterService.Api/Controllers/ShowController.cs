using Azure.Core;
using BookingService.Application.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Shows.Requests;
using TheatreMasterService.Application.DTOs.Shows.Responses;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for movie show schedules, timings, pricing, and seat availability.
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class ShowController(IShowService service, ILogger<ShowController> logger) : ControllerBase
    {
        private readonly IShowService _service = service;
        private readonly ILogger<ShowController> _logger = logger;

        /// <summary>
        /// Retrieves all movie show schedules.
        /// </summary>
        /// <returns>A collection of show schedules.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Show>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShows()
        {
            try
            {
                var shows = await _service.GetShowsAsync();
                return Ok(new
                {
                    success = true,
                    message = "Shows retrieved successfully.",
                    data = shows
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all shows.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving shows.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves a specific show schedule by identifier.
        /// </summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>The matching show object.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Show), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowById(Guid id)
        {
            try
            {
                var show = await _service.GetShowByIdAsync(id);
                if (show == null)
                {
                    return NotFound(new { success = false, message = $"Show with ID {id} was not found." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Show retrieved successfully.",
                    data = show
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the show.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a new movie show schedule. (Admin role required)
        /// </summary>
        /// <param name="show">The show entity payload.</param>
        /// <returns>The created show.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Show), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddShow([FromBody] CreateShowRequest request)
        {
            try
            {
                var createdShow = await _service.CreateShowAsync(request);
                return CreatedAtAction(nameof(GetShowById), new { id = createdShow.ShowId }, new
                {
                    success = true,
                    message = "Show created successfully.",
                    data = createdShow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new show schedule.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while creating the show.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing movie show schedule. (Admin role required)
        /// </summary>`
        /// <param name="id">The show identifier.</param>
        /// <param name="show">The updated show payload.</param>
        /// <returns>True if update was successful.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShow(Guid id, [FromBody] UpdateShowRequest request)
        {
            try
            {
                var updated = await _service.UpdateShowAsync(id, request);
                if (!updated)
                    return NotFound(new { success = false, message = $"Show with ID {id} was not found for update." });

                return Ok(new
                {
                    success = true,
                    message = "Show updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the show.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a show schedule by identifier. (Admin role required)
        /// </summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>True if deletion was successful.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShow(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteShowAsync(id);
                if (!deleted)
                    return NotFound(new { success = false, message = $"Show with ID {id} was not found for deletion." });

                return Ok(new
                {
                    success = true,
                    message = "Show deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the show.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves all show schedules for a specific movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of shows for the movie.</returns>
        [HttpGet("movie/{movieId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<Show>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowsByMovieId(Guid movieId)
        {
            try
            {
                var shows = await _service.GetShowsByMovieIdAsync(movieId);
                return Ok(new
                {
                    success = true,
                    message = "Shows retrieved successfully for the movie.",
                    data = shows
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving shows for Movie ID {MovieId}.", movieId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving movie shows.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves available seat count for a specific show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>The number of available seats.</returns>
        [HttpGet("{showId:guid}/available-seats")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableSeats(Guid showId)
        {
            try
            {
                var seats = await _service.GetAvailableSeatsAsync(showId);
                return Ok(new
                {
                    success = true,
                    message = "Available seats retrieved successfully.",
                    data = new { showId, availableSeats = seats }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving available seats for Show ID {ShowId}.", showId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving available seats.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Returns the seating layout for the screen hosting the given show.
        /// </summary>
        [HttpGet("{showId:guid}/screen-layout")]
        [ProducesResponseType(typeof(ApiResponse<ScreenLayoutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreenLayout(Guid showId)
        {
            try
            {
                var layout = await _service.GetScreenLayoutAsync(showId);
                return Ok(new
                {
                    success = true,
                    message = "Screen layout retrieved successfully.",
                    data = layout
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching screen layout for show {ShowId}.", showId);
                return StatusCode(500, new { success = false, message = "An error occurred.", details = ex.Message });
            }
        }
    }
}