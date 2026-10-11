using LeagueTracker.Application.DTOs.Stats;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IPlayerStatsService _playerStatsService;

        public PlayerStatsController(IPlayerStatsService playerStatsService)
        {
            _playerStatsService = playerStatsService;
        }

        [HttpGet]
        public async Task<ActionResult<PlayerStatsTableDto>> GetAsync([FromQuery] int seasonId)
        {
            var table = await _playerStatsService.GetStatsAsync(seasonId);
            return Ok(table);

        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<PlayerStatsDetailDto>> GetPlayerDetail(int playerId, [FromQuery] int? seasonId)
        {
            return Ok(await _playerStatsService.GetPlayerDetailAsync(playerId, seasonId));
        }
    }
}
