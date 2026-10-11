
using LeagueTracker.Application.DTOs.Auth;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";

    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result is null)
        {
            return BadRequest("Registracija nije uspela.");
        }

        SetRefreshCookie(result);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
        {
            return Unauthorized("Pogrešan email ili lozinka.");
        }

        SetRefreshCookie(result);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh()
    {
        var cookieCount = Request.Headers.Cookie.ToString().Split("refresh_token=").Length - 1;
        _logger.LogInformation("Refresh: broj refresh_token cookie-ja u zahtevu: {Count}", cookieCount);
        var token = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Refresh: cookie nije stigao do servera");
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(token);
        if (result is null)
        {
            return Unauthorized();
        }

        SetRefreshCookie(result);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(Request.Cookies[RefreshCookieName]);
        ClearRefreshCookie();
        return NoContent();
    }

    private void SetRefreshCookie(AuthResultDto result)
    {
        Response.Cookies.Append(RefreshCookieName, result.RefreshToken!, CookieOptionsFor(result.RefreshTokenExpiresAt));
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Delete(RefreshCookieName, CookieOptionsFor(null));
    }

    private CookieOptions CookieOptionsFor(DateTime? expiresAt) => new()
    {
        HttpOnly = true,                    // JavaScript ne može da pročita cookie
        Secure = Request.IsHttps,           // u produkciji (HTTPS) uvek true
        SameSite = SameSiteMode.Strict,     // cookie se ne šalje sa tuđih sajtova
        Path = "/api/Auth",                 // šalje se samo ka auth endpoint-ima
        Expires = expiresAt.HasValue ? new DateTimeOffset(expiresAt.Value, TimeSpan.Zero) : null
    };
}
