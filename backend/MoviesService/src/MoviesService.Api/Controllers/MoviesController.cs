using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesService.Application.Abstractions;
using MoviesService.Application.DTOs.Request;
using MoviesService.Domain.Entities;

namespace MoviesService.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing movie catalog records, administrative updates, and content lifecycle.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController(IMoviesService movieservice) : ControllerBase
    {
        #region configuration
        private readonly IMoviesService _movieservice = movieservice;
        #endregion

        /// <summary>
        /// Retrieves all available movies in the catalog.
        /// </summary>
        /// <returns>A list of movies.</returns>
        /// <response code="200">Returns the complete list of movies.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Movies>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await _movieservice.GetMoviesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Movies retrieved successfully.",
                    data = movies
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving movies.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves a specific movie by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the movie.</param>
        /// <returns>The movie details if found.</returns>
        /// <response code="200">Returns the requested movie.</response>
        /// <response code="404">Returned if the movie does not exist.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Movies), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            try
            {
                var movie = await _movieservice.GetMoviesByIdAsync(id);
                if (movie == null)
                    return NotFound(new { success = false, message = "Movie not found." });

                return Ok(new
                {
                    success = true,
                    message = "Movie retrieved successfully.",
                    data = movie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the movie.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves the poster image file for a specific movie using a structured request payload.
        /// </summary>
        /// <param name="id">The unique identifier of the movie.</param>
        /// <returns>The poster image file stream.</returns>
        /// <response code="200">Returns the poster image binary stream.</response>
        /// <response code="404">Returned if the movie or poster does not exist.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [AllowAnonymous]
        [HttpGet("{id:guid}/poster")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMoviePoster(Guid id)
        {
            try
            {
                var request = new GetMoviePosterRequest { MovieId = id };
                var response = await _movieservice.GetMoviePosterAsync(request);

                return File(response.FileBytes, response.ContentType);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the movie poster.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a new movie entry in the system (Admin only).
        /// </summary>
        /// <param name="dto">The movie creation payload containing metadata and optional poster image file.</param>
        /// <returns>The newly created movie entity.</returns>
        /// <response code="200">Returns the created movie record.</response>
        /// <response code="400">Returned if validation fails.</response>
        /// <response code="401">Returned if the user is unauthenticated.</response>
        /// <response code="403">Returned if the user does not have Administrator privileges.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Movies), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddMovie([FromForm] MovieCreateUpdateDto dto)
        {
            try
            {
                var createdMovie = await _movieservice.CreateMoviesAsync(dto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Movie created successfully.",
                    data = createdMovie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while creating the movie.",
                    details = ex.Message
                });
            }
        }

        /// Updates an existing movie record by its identifier (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the movie to update.</param>
        /// <param name="dto">The updated movie details and optional new poster image file.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Movie updated successfully.</response>
        /// <response code="400">Returned if validation fails.</response>
        /// <response code="404">Returned if the movie does not exist.</response>
        /// <response code="401">Returned if the user is unauthenticated.</response>
        /// <response code="403">Returned if the user does not have Administrator privileges.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromForm] MovieCreateUpdateDto dto)
        {
            try
            {
                var existingMovie = await _movieservice.GetMoviesByIdAsync(id);
                if (existingMovie == null)
                    return NotFound(new { message = "Movie not found." });

                var updatedMovie = await _movieservice.UpdateMoviesAsync(id, dto);
                if (updatedMovie == null)
                    return BadRequest(new { success = false, message = "Failed to update the movie. Please check your data and try again." });

                return Ok(new
                {
                    success = true,
                    message = "Movie updated successfully.",
                    data = updatedMovie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the movie.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a movie by its identifier, ensuring no active shows or bookings are tied to it (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the movie to delete.</param>
        /// <returns>Confirmation message upon successful deletion.</returns>
        /// <response code="200">Movie deleted successfully.</response>
        /// <response code="400">Returned if the movie has active shows or bookings preventing deletion.</response>
        /// <response code="404">Returned if the movie does not exist.</response>
        /// <response code="401">Returned if the user is unauthenticated.</response>
        /// <response code="403">Returned if the user does not have Administrator privileges.</response>
        /// <response code="500">Returned if an unhandled server error occurs.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            try
            {
                var existingMovie = await _movieservice.GetMoviesByIdAsync(id);
                if (existingMovie == null)
                    return NotFound(new { success = false, message = "Movie not found." });

                await _movieservice.DeleteMoviesAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "Movie deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the movie.",
                    details = ex.Message
                });
            }
        }
    }
}