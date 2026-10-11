using LeagueTracker.Application.DTOs.Standings;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService _standingsService;

        public StandingsController(IStandingsService standingsService)
        {
            _standingsService = standingsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StandingsRowDto>>> Get([FromQuery] int seasonId)
        {
            var standings = await _standingsService.GetStandingsAsync(seasonId);
            return Ok(standings);
        }
    }
}
