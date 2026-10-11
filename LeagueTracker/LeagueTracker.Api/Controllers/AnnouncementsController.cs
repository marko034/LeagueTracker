using LeagueTracker.Application.DTOs.Announcement;
using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;

    public AnnouncementsController(IAnnouncementService announcementService)
    {
        _announcementService = announcementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetRecent()
    {
        return Ok(await _announcementService.GetRecentAsync());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> Create(CreateAnnouncementDto dto)
    {
        return Ok(await _announcementService.CreateAsync(dto));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _announcementService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
