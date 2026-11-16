using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for cinema screen operations, auditoriums, and capacities.
    /// </summary>
    [Route("/api/[controller]/")]
    [ApiController]
    public class ScreenController(IScreenService service, ILogger<ScreenController> logger) : ControllerBase
    {
        private readonly IScreenService _service = service;
        private readonly ILogger<ScreenController> _logger = logger;

        /// <summary>
        /// Retrieves all configured screens.
        /// </summary>
        /// <returns>A collection of all screens.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Screen>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreens()
        {
            try
            {
                var screens = await _service.GetScreensAsync();
                return Ok(screens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all screens.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a specific screen by its unique identifier.
        /// </summary>
        /// <param name="id">The screen identifier.</param>
        /// <returns>The matching screen object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Screen), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreenById(int id)
        {
            try
            {
                var screen = await _service.GetScreenByIdAsync(id);
                if (screen == null)
                {
                    return NotFound($"Screen with ID {id} was not found.");
                }
                return Ok(screen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Creates a new screen record. (Admin role required)
        /// </summary>
        /// <param name="screen">The screen entity payload.</param>
        /// <returns>The newly created screen.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Screen), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddScreen([FromBody] Screen screen)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdScreen = await _service.CreateScreenAsync(screen);
                return CreatedAtAction(nameof(GetScreenById), new { id = createdScreen.ScreenId }, createdScreen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new screen.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing screen record. (Admin role required)
        /// </summary>
        /// <param name="id">The screen identifier.</param>
        /// <param name="screen">The updated screen data.</param>
        /// <returns>True if the update was successful.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateScreen(int id, [FromBody] Screen screen)
        {
            try
            {
                var result = await _service.UpdateScreenAsync(id, screen);
                if (!result)
                {
                    return NotFound($"Screen with ID {id} was not found for update.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Deletes a screen record by identifier. (Admin role required)
        /// </summary>
        /// <param name="id">The screen identifier.</param>
        /// <returns>True if the deletion was successful.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            try
            {
                var result = await _service.DeleteScreenAsync(id);
                if (!result)
                {
                    return NotFound($"Screen with ID {id} was not found for deletion.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }
    }
}