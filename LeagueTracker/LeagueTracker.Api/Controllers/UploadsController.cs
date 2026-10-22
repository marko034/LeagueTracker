using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private const long MaxBytes = 5 * 1024 * 1024;

    private readonly IImageService _imageService;

    public UploadsController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [RequestSizeLimit(MaxBytes + 512 * 1024)]
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("Fajl je prazan.");
        }

        if (file.Length > MaxBytes)
        {
            return BadRequest("Fajl je prevelik (max 5 MB).");
        }

        await using var stream = file.OpenReadStream();
        var path = await _imageService.ProcessAndSaveAsync(stream, cancellationToken);

        return Ok(new { path });
    }
}