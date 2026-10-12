
using LeagueTracker.Application.DTOs.Auth;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using LeagueTracker.Infrastructure.Identity;
using LeagueTracker.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly int _absoluteSessionDays;
    private readonly JwtSettings _jwt;

    public AuthService(UserManager<ApplicationUser> usermanager, ITokenService tokenService, LeagueTrackerDbContext context, ILogger<AuthService> logger, IOptions<JwtSettings> jwtOptions)
    {
        _userManager = usermanager;
        _tokenService = tokenService;
        _context = context;
        _jwt = jwtOptions.Value;
        _refreshTokenDays = _jwt.RefreshTokenDays;
        _logger = logger;
        _absoluteSessionDays = _jwt.AbsoluteSessionDays;
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

        var familyId = await _context.RefreshTokens
        .Where(t => t.TokenHash == hash)
        .Select(t => (Guid?)t.FamilyId)
        .FirstOrDefaultAsync();

        if (familyId.HasValue)
        {
            await RevokeFamilyAsync(familyId.Value);
        }
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var hash = Hash(refreshToken);

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
                await RevokeFamilyAsync(stored.FamilyId);
            }
            return null;
        }

        var now = DateTime.UtcNow;

        if(stored.ExpiresAt <= now || stored.SessionStartedAt.AddDays(_absoluteSessionDays) < now)
        {
            _logger.LogWarning("Refresh odbijen: istekao(token { Id}, sesija { FamilyId})", stored.Id, stored.FamilyId);
            return null;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        // "Potroši" token atomično: uspeva samo za jedan od istovremenih zahteva
        var consumed = await _context.RefreshTokens
            .Where(t => t.Id == stored.Id && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));

        if (consumed == 0)
        {
            return null; // neko je u međuvremenu iskoristio isti token
        }

        var result = await IssueTokenAsync(stored.User, stored.FamilyId, stored.SessionStartedAt);
        await transaction.CommitAsync();

        return result;
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

    private async Task<AuthResultDto> IssueTokenAsync(ApplicationUser user, Guid? familyId = null, DateTime? sessionStartedAt =null)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, user.DisplayName, roles);
        var (refreshToken, refreshExpiresAt) = await CreateRefreshTokenAsync(user.Id, familyId ?? Guid.NewGuid(), sessionStartedAt ?? DateTime.UtcNow);

        return new AuthResultDto
        {
            Token = accessToken,
            DisplayName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "",
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private async Task<(string refreshToken, DateTime refreshExpiresAt)> CreateRefreshTokenAsync(string id, Guid familyId, DateTime sessionStartedAt)
    {
        var now = DateTime.UtcNow;
        var raw = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var hash = Hash(raw);

        var slidingEnd = now.AddDays(_refreshTokenDays);
        var absoluteEnd = sessionStartedAt.AddDays(_absoluteSessionDays);
        var expiresAt = slidingEnd < absoluteEnd ? slidingEnd : absoluteEnd;

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = id,
            TokenHash = hash,
            CreatedAt = now,
            ExpiresAt = expiresAt,
            FamilyId = familyId,
            SessionStartedAt = sessionStartedAt
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

    private Task<int> RevokeFamilyAsync(Guid familyId) =>
    _context.RefreshTokens
        .Where(t => t.FamilyId == familyId && t.RevokedAt == null)
        .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTime.UtcNow));

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
