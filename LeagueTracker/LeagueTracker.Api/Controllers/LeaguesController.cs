using LeagueTracker.Application.DTOs.Leagues;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeagueDto>>> GetAll() => Ok(await _leagueService.GetAllAsync());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LeagueDto>> Create(CreateLeagueDto dto) => Ok(await _leagueService.CreateAsync(dto));
}
