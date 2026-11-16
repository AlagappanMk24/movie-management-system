using Microsoft.AspNetCore.Mvc;
using UserService.Application.Abstractions;
using UserService.Application.DTOs;

namespace UserService.Api.Controllers;

/// <summary>
/// Controller responsible exclusively for HTTP request routing, input parsing, and response formatting.
/// </summary>
/// <remarks>
/// <b>Patterns Used:</b> Thin Controller + Delegation.<br/>
/// <b>Purpose:</b> Accepts HTTP traffic, delegates processing to application services, and maps results to standard HTTP status codes.<br/>
/// <b>Why It Is Used:</b> Keeps business rules and security generation out of controllers, ensuring separation of concerns and robust error handling.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    ITokenService tokenService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ITokenService _tokenService = tokenService;

    /// <summary>
    /// Authenticates user credentials and returns a JWT access token.
    /// </summary>
    /// <response code="200">Returns the generated JWT token and user profile details.</response>
    /// <response code="401">Returned when email or password validation fails.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        try
        {
            var user = await _authService.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = _tokenService.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            // Global/Local fallback handling; production logging middleware would capture this automatically.
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing login.", details = ex.Message });
        }
    }

    /// <summary>
    /// Registers a new user account in the system.
    /// </summary>
    /// <response code="200">Returns the newly created user details.</response>
    /// <response code="400">Returned if the user already exists or validation fails.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        try
        {
            var user = await _authService.RegisterUserAsync(request);

            if (user == null)
            {
                return BadRequest(new { message = "User with this email already exists." });
            }

            return Ok(new
            {
                user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during registration.", details = ex.Message });
        }
    }
}