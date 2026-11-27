using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Request;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Api.Controllers
{
    /// <summary>
    /// Manages API endpoints for cinema screen operations, auditoriums, and capacities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenController(IScreenService service, ILogger<ScreenController> logger) : ControllerBase
    {
        private readonly IScreenService _service = service;
        private readonly ILogger<ScreenController> _logger = logger;

        #region Get All Screens
        /// <summary>
        /// Retrieves all configured screens.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreens()
        {
            try
            {
                var screens = await _service.GetScreensAsync();
                return Ok(new
                {
                    success = true,
                    message = "Screens retrieved successfully.",
                    data = screens
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all screens.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving screens.",
                    details = ex.Message
                });
            }
        }
        #endregion

        #region Get Screen By ID
        /// <summary>
        /// Retrieves a specific screen by its unique identifier.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScreenById(Guid id)
        {
            try
            {
                var screen = await _service.GetScreenByIdAsync(id);
                if (screen == null)
                {
                    return NotFound(new { success = false, message = $"Screen with ID {id} was not found or its parent theatre is unavailable." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Screen retrieved successfully.",
                    data = screen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the screen.",
                    details = ex.Message
                });
            }
        }
        #endregion

        #region Create Screen
        /// <summary>
        /// Creates a new screen record. (Admin role required)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddScreen([FromBody] ScreenCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
                }

                var createdScreen = await _service.CreateScreenAsync(dto);
                return CreatedAtAction(nameof(GetScreenById), new { id = createdScreen.Id }, new
                {
                    success = true,
                    message = "Screen created successfully.",
                    data = createdScreen
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid theatre reference {TheatreId} while creating screen.", dto.TheatreId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new screen.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while creating the screen.",
                    details = ex.Message
                });
            }
        }
        #endregion

        #region Update Screen
        /// <summary>
        /// Updates an existing screen record. (Admin role required)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateScreen(Guid id, [FromBody] ScreenCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
                }

                var updatedScreen = await _service.UpdateScreenAsync(id, dto);
                if (updatedScreen == null)
                {
                    return NotFound(new { success = false, message = $"Screen with ID {id} was not found for update." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Screen updated successfully.",
                    data = updatedScreen
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid theatre reference {TheatreId} while updating screen.", dto.TheatreId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the screen.",
                    details = ex.Message
                });
            }
        }
        #endregion

        #region Delete Screen
        /// <summary>
        /// Deletes a screen record by identifier. (Admin role required)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScreen(Guid id)
        {
            try
            {
                var result = await _service.DeleteScreenAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = $"Screen with ID {id} was not found for deletion." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Screen deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting screen with ID {ScreenId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the screen.",
                    details = ex.Message
                });
            }
        }
        #endregion
    }
}