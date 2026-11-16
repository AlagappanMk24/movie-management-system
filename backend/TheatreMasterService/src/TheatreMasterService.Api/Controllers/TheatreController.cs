using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for theatre complex operations, locations, and details.
    /// </summary>
    [Route("/api/[controller]/")]
    [ApiController]
    public class TheatreController(ITheatreService theatreService, ILogger<TheatreController> logger) : ControllerBase
    {
        private readonly ITheatreService _theatreservice = theatreService;
        private readonly ILogger<TheatreController> _logger = logger;

        /// <summary>
        /// Retrieves all theatre complexes.
        /// </summary>
        /// <returns>A collection of theatres.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Theatre>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTheatre()
        {
            try
            {
                var theatres = await _theatreservice.GetTheatresAsync();
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all theatres.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Creates a new theatre complex record. (Admin role required)
        /// </summary>
        /// <param name="theatre">The theatre entity payload.</param>
        /// <returns>The created theatre.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Theatre), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTheatre([FromBody] Theatre theatre)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTheatre = await _theatreservice.CreateTheatreAsync(theatre);
                return CreatedAtAction(nameof(GetTheatreById), new { id = createdTheatre.TheatreId }, createdTheatre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new theatre.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a specific theatre by identifier.
        /// </summary>
        /// <param name="id">The theatre identifier.</param>
        /// <returns>The matching theatre object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Theatre), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTheatreById(int id)
        {
            try
            {
                var theatre = await _theatreservice.GetTheatreByIdAsync(id);
                if (theatre == null)
                {
                    return NotFound($"Theatre with ID {id} was not found.");
                }
                return Ok(theatre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving theatre with ID {TheatreId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing theatre complex. (Admin role required)
        /// </summary>
        /// <param name="id">The theatre identifier.</param>
        /// <param name="theatre">The updated theatre payload.</param>
        /// <returns>True if update was successful.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTheatre(int id, [FromBody] Theatre theatre)
        {
            try
            {
                var result = await _theatreservice.UpdateTheatreAsync(id, theatre);
                if (!result)
                {
                    return NotFound($"Theatre with ID {id} was not found for update.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating theatre with ID {TheatreId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Deletes a theatre complex by identifier. (Admin role required)
        /// </summary>
        /// <param name="id">The theatre identifier.</param>
        /// <returns>True if deletion was successful.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTheatre(int id)
        {
            try
            {
                var result = await _theatreservice.DeleteTheatreAsync(id);
                if (!result)
                {
                    return NotFound($"Theatre with ID {id} was not found for deletion.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting theatre with ID {TheatreId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }
    }
}