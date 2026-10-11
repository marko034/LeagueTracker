using LeagueTracker.Application.Interfaces;
using LeagueTracker.Application.Seasons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeasonsController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SeasonDto>>> GetAll()
    {
        return Ok(await _seasonService.GetAllAsync());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<SeasonDto>> Create(CreateSeasonDto dto)
    {
        var created = await _seasonService.CreateAsync(dto);
        return Ok(created);
    }
}
