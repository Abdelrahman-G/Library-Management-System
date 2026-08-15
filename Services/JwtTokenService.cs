using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Library_Management_System.Services.Interfaces;
using Library_Management_System.Services.Results;
using Microsoft.IdentityModel.Tokens;

namespace Library_Management_System.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResult CreateToken(SystemUser user)
    {
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");

        var encodedKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured.");

        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes");

        if (expirationMinutes <= 0)
            throw new InvalidOperationException("JWT expiration must be greater than zero.");

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(encodedKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "JWT signing key must be a valid Base64 value.",
                exception);
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.SystemUserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(user.SystemUserRoles
            .Select(userRole => userRole.Role.RoleName)
            .Distinct()
            .Select(roleName => new Claim(ClaimTypes.Role, roleName)));

        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(expirationMinutes);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new TokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
