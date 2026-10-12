

using Microsoft.Extensions.Options;

namespace LeagueTracker.Infrastructure.Settings;

public class JwtSettingsValidator : IValidateOptions<JwtSettings>
{
    public ValidateOptionsResult Validate(string? name, JwtSettings options) =>
        options.AbsoluteSessionDays < options.RefreshTokenDays
            ? ValidateOptionsResult.Fail("AbsoluteSessionDays ne sme biti manji od RefreshTokenDays.")
        : ValidateOptionsResult.Success;
}
