

using System.ComponentModel.DataAnnotations;

namespace LeagueTracker.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required, MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiryMinutes { get; set; } = 15;
    [Range(1, 365)]
    public int RefreshTokenDays { get; set; } = 7;
    [Range(1, 365)]
    public int AbsoluteSessionDays { get; set; } = 30;
}
