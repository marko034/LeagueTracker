
using LeagueTracker.Application.DTOs.Auth;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using LeagueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LeagueTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly LeagueTrackerDbContext _context;
    private readonly int _refreshTokenDays;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> usermanager, ITokenService tokenService, LeagueTrackerDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userManager = usermanager;
        _tokenService = tokenService;
        _context = context;
        _refreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenDays"] ?? "7");
        _logger = logger;
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if(user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return null;
        }

        return await IssueTokenAsync(user);
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return;
        }

        var hash = Hash(refreshToken);

        await _context.RefreshTokens
            .Where(t => t.TokenHash == hash && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var hash = Hash(refreshToken);

        _logger.LogInformation("Refresh: dužina {Length}, hash {HashPrefix}", refreshToken.Length, hash[..8]);

        var stored = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash);

        if (stored is null)
        {
            _logger.LogWarning("Refresh odbijen: token nije pronađen u bazi");
            return null;
        }
        
        if(stored.RevokedAt is not null)
        {
            _logger.LogWarning("Refresh odbijen: token nije pronađen u bazi");

            if (stored.RevokedAt < DateTime.UtcNow.AddSeconds(-10))
            {
                await RevokeAllAsync(stored.UserId);
            }
            return null;
        }

        if(stored.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh odbijen: token je istekao u {ExpiresAt}", stored.ExpiresAt);
            return null;
        }

        stored.RevokedAt = DateTime.UtcNow;
        return await IssueTokenAsync(stored.User);
    }

    public async Task<AuthResultDto?> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PlayerId = dto.PlayerId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return null;
        }

        await _userManager.AddToRoleAsync(user, "Competitor");

        return await IssueTokenAsync(user);
    }

    private async Task<AuthResultDto> IssueTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, user.DisplayName, roles);
        var (refreshToken, refreshExpiresAt) = await CreateRefreshTokenAsync(user.Id);

        return new AuthResultDto
        {
            Token = accessToken,
            DisplayName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "",
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private async Task<(string refreshToken, DateTime refreshExpiresAt)> CreateRefreshTokenAsync(string id)
    {
        var raw = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        var hash = Hash(raw);
        var expiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = id,
            TokenHash = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        });

        await _context.SaveChangesAsync();
        _logger.LogInformation("Izdat refresh token: dužina {Length}, hash {HashPrefix}", raw.Length, hash[..8]);
        return (raw, expiresAt);
    }

    private async Task RevokeAllAsync(string userId)
    {
        await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
    }

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
