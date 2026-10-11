
using LeagueTracker.Application.DTOs.Games;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GameDto>> Record(CreateGameDto gameDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var created = await _gameService.RecordGameAsync(gameDto, userId, isAdmin);
        return Ok(created);
    }

    [Authorize]
    [HttpPost("finalize-match/{matchId}")]
    public async Task<IActionResult> FinalizeMatch(int matchId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var result = await _gameService.FinalizeMatchAsync(matchId, userId, isAdmin);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("verify-match/{matchId}")]
    public async Task<IActionResult> VerifyMatch(int matchId)
    {
        var success = await _gameService.VerifyMatchAsync(matchId);
        return success ? Ok() : BadRequest("Meč nije u stanju koje se može verifikovati.");
    }
}
