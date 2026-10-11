
using LeagueTracker.Application.DTOs.Matches;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MatchDto>>> GetBySeason([FromQuery] int seasonId)
    {
        var matches = await _matchService.GetBySeasonAsync(seasonId);
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDto>> GetById(int id)
    {
        var match = await _matchService.GetByIdAsync(id);
        return match is null ? NotFound() : Ok(match);
    }

    [HttpPost]
    public async Task<ActionResult<MatchDto>> Create(CreateMatchDto dto)
    {
        var created = await _matchService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("awaiting-verification")]
    public async Task<ActionResult<List<MatchDto>>> GetAwaitingVerification()
    {
        var matches = await _matchService.GetAwaitingVerificationAsync();
        return Ok(matches);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _matchService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
