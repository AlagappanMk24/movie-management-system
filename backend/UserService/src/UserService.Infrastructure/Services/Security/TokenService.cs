using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services.Security;

/// <summary>
/// Handles cryptographic generation of JSON Web Tokens.
/// </summary>
/// <remarks>
/// <b>Patterns Used:</b> Options Pattern (via IConfiguration) + Cryptographic Signing.<br/>
/// <b>Purpose:</b> Creates secure, claims-based authentication tokens for API consumers.<br/>
/// <b>Why It Is Used:</b> Encapsulates security algorithms, secret keys, and expiration policies safely within the infrastructure layer.
/// </remarks>
public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Generates a signed JWT token containing user identity claims, with built-in exception safeguards for missing configuration keys.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration keys are missing or unconfigured.</exception>
    public string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:Key"]
            ?? throw new InvalidOperationException("JWT Secret Key is not configured in application settings.");

        var issuer = _configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured in application settings.");

        var audience = _configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is not configured in application settings.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),
            new Claim("UserId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}