

using LeagueTracker.Application.DTOs.Auth;

namespace LeagueTracker.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResultDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string? refreshToken);
}
