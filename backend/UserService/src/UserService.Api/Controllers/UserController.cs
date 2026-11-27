using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Api.Controllers
{
    /// <summary>
    /// Manages user accounts, profile updates, and administrative user queries.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService service, ILogger<UserController> logger) : ControllerBase
    {
        #region Configuration
        private readonly IUserService _service = service;
        private readonly ILogger<UserController> _logger = logger;
        #endregion

        #region GetUser
        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>A collection of users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">Returned if the caller is not authenticated.</response>
        /// <response code="403">Returned if the caller does not have the Admin role.</response>
        /// <response code="500">Returned if an unexpected error occurs while retrieving users.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var users = await _service.GetUserAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving users."
                });
            }
        }
        #endregion

        #region CreateUser
        /// <summary>
        /// Creates a new user record.
        /// </summary>
        /// <param name="user">The user payload to create.</param>
        /// <returns>The created user details.</returns>
        /// <response code="200">Returns the newly created user.</response>
        /// <response code="400">Returned if the request payload is invalid or violates a business rule.</response>
        /// <response code="401">Returned if the caller is not authenticated.</response>
        /// <response code="409">Returned if a conflicting user record already exists.</response>
        /// <response code="500">Returned if an unexpected error occurs while creating the user.</response>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                var createdUser = await _service.CreateUserAsync(user);
                return Ok(createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user payload during creation.");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict while creating user.");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while creating the user."
                });
            }
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// Updates an existing user by identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="user">The updated user details.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the updated user.</response>
        /// <response code="400">Returned if the request payload or identifier is invalid.</response>
        /// <response code="401">Returned if the caller is not authenticated.</response>
        /// <response code="404">Returned if no user is found with the specified identifier.</response>
        /// <response code="500">Returned if an unexpected error occurs while updating the user.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            try
            {
                var updatedUser = await _service.UpdateUserAsync(id, user);
                if (updatedUser)
                {
                    return NotFound(new { success = false, message = $"User with ID '{id}' was not found." });
                }

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid update request for user ID {UserId}.", id);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User ID {UserId} not found during update.", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the user."
                });
            }
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// Deletes an existing user by identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        /// <response code="200">User was deleted successfully.</response>
        /// <response code="401">Returned if the caller is not authenticated.</response>
        /// <response code="404">Returned if no user is found with the specified identifier.</response>
        /// <response code="500">Returned if an unexpected error occurs while deleting the user.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteUserAsync(id);
                if (!deleted)
                {
                    return NotFound(new { success = false, message = $"User with ID '{id}' was not found." });
                }

                return Ok(deleted);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User ID {UserId} not found during deletion.", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the user."
                });
            }
        }
        #endregion

        #region GetUserById
        /// <summary>
        /// Retrieves a user by unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The matching user record.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="404">Returned if no user is found with the specified identifier.</response>
        /// <response code="500">Returned if an unexpected error occurs while retrieving the user.</response>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user is null)
                {
                    return NotFound(new { success = false, message = $"User with ID '{id}' was not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the user."
                });
            }
        }
        #endregion
    }
}