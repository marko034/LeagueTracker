using LeagueTracker.Application.DTOs.Teams;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamDto>>> Get([FromQuery] int? seasonId) => Ok(await _teamService.GetAsync(seasonId));

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            return team is null ? NotFound() : Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> Create(CreateTeamDto createTeamDto)
        {
            var created = await _teamService.CreateAsync(createTeamDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _teamService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("id")]
        public async Task<ActionResult<TeamDto>> Update(int id, CreateTeamDto teamDto)
        {
            var updated = await _teamService.UpdateAsync(id, teamDto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/seasons/{seasonId}")]
        public async Task<IActionResult> RegisterToSeason(int id, int seasonId) 
            => await _teamService.RegisterToSeasonAsync(id, seasonId) ? Ok() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/seasons/{seasonId}")]
        public async Task<IActionResult> UnregisterFromSeason(int id, int seasonId)
            => await _teamService.UnregisterFromSeasonAsync(id, seasonId) ? NoContent() : NotFound();

        [HttpGet("{id}/history")]
         public async Task<ActionResult<List<TeamSeasonHistoryDto>>> GetHistory(int id)
            => Ok(await _teamService.GetHistoryAsync(id));

    }
}
