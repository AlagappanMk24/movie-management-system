using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for movie show schedules, timings, pricing, and seat availability.
    /// </summary>
    [Route("/api/[controller]/")]
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
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all shows.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a specific show schedule by identifier.
        /// </summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>The matching show object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Show), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowById(int id)
        {
            try
            {
                var show = await _service.GetShowByIdAsync(id);
                if (show == null)
                {
                    return NotFound($"Show with ID {id} was not found.");
                }
                return Ok(show);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
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
        public async Task<IActionResult> AddShow([FromBody] Show show)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdShow = await _service.CreateShowAsync(show);
                return CreatedAtAction(nameof(GetShowById), new { id = createdShow.ShowId }, createdShow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new show schedule.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing movie show schedule. (Admin role required)
        /// </summary>
        /// <param name="id">The show identifier.</param>
        /// <param name="show">The updated show payload.</param>
        /// <returns>True if update was successful.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] Show show)
        {
            try
            {
                var result = await _service.UpdateShowAsync(id, show);
                if (!result)
                {
                    return NotFound($"Show with ID {id} was not found for update.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Deletes a show schedule by identifier. (Admin role required)
        /// </summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>True if deletion was successful.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShow(int id)
        {
            try
            {
                var result = await _service.DeleteShowAsync(id);
                if (!result)
                {
                    return NotFound($"Show with ID {id} was not found for deletion.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting show with ID {ShowId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Retrieves all show schedules for a specific movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of shows for the movie.</returns>
        [HttpGet("movie/{movieId}")]
        [ProducesResponseType(typeof(IEnumerable<Show>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShowsByMovieId(int movieId)
        {
            try
            {
                var shows = await _service.GetShowsByMovieId(movieId);
                return Ok(shows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving shows for Movie ID {MovieId}.", movieId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Retrieves available seat count for a specific show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>The number of available seats.</returns>
        [HttpGet("{showId}/available-seats")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableSeats(int showId)
        {
            try
            {
                var seats = await _service.GetAvailableSeatsAsync(showId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving available seats for Show ID {ShowId}.", showId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }
    }
}