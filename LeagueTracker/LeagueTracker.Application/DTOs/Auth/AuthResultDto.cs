

using System.Text.Json.Serialization;

namespace LeagueTracker.Application.DTOs.Auth;

public class AuthResultDto
{
    public string Token { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    [JsonIgnore] public string? RefreshToken { get; set; }
    [JsonIgnore] public DateTime RefreshTokenExpiresAt { get; set; }
}
