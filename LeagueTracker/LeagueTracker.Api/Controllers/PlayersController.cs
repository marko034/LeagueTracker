using LeagueTracker.Application.DTOs.Players;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> Get([FromQuery] int? teamId, [FromQuery] int? seasonId)
        => Ok(await _playerService.GetAsync(teamId, seasonId));

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(int id, [FromQuery] int? seasonId)
    {
        var player = await _playerService.GetByIdAsync(id,seasonId);
        return player is null ? NotFound() : Ok(player);
    }

    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create(CreatePlayerDto player)
    {
        var created = await _playerService.CreateAsync(player);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _playerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("id")]
    public async Task<ActionResult<PlayerDto>> Update(int id, CreatePlayerDto playerDto)
    {
        var updated = await _playerService.UpdateAsync(id, playerDto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(int id, RegisterPlayerDto dto)
        => await _playerService.RegisterAsync(id, dto) ? Ok() : NotFound();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/register")]
    public async Task<IActionResult> Unregister(int id, [FromQuery] int seasonId)
        => await _playerService.UnregisterAsync(id, seasonId) ? NoContent() : NotFound();

}
