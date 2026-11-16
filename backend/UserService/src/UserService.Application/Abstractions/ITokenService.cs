namespace UserService.Application.Abstractions;

/// <summary>
/// Defines contract for token generation services.
/// </summary>
/// <remarks>
/// <b>Pattern Used:</b> Service Abstraction.<br/>
/// <b>Purpose:</b> Decouples token creation from controllers and infrastructure.<br/>
/// <b>Why It Is Used:</b> Keeps the API layer thin and clean by abstracting security concerns away from HTTP routing.
/// </remarks>
public interface ITokenService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) for the authenticated user.
    /// </summary>
    /// <param name="user">The user entity containing claims information.</param>
    /// <returns>A signed JWT string.</returns>
    string GenerateJwtToken(Domain.Entities.User user);
}