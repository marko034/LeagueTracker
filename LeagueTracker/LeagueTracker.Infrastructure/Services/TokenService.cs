

using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeagueTracker.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> options)
    {
        _jwt = options.Value;
    }
    public string GenerateToken(string userId, string email, string displayName, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId),
            new (ClaimTypes.Email, email),
            new (ClaimTypes.Name, displayName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: credentials
         );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
