
using LeagueTracker.Application.DTOs.Sports;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SportsController : ControllerBase
{
    private readonly ISportService _sportService;

    public SportsController(ISportService sportService)
    {
        _sportService = sportService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SportDto>>> GetAll()
    {
        var sports =  await _sportService.GetAllAsync();
        return Ok(sports);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SportDto>> GetById(int id)
    {
        var sport = await _sportService.GetByIdAsync(id);
        return sport is null ? NotFound() : Ok(sport);
    }

    [HttpPost]
    public async Task<ActionResult<CreateSportDto>> Create(CreateSportDto createSportDto)
    {
        var createdSport = await _sportService.CreateAsync(createSportDto);
        return CreatedAtAction(nameof(GetById), new { id = createdSport.Id }, createdSport);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _sportService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
