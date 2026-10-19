using LeagueTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxBytes = 5 * 1024 * 1024;

    private readonly IFileStorage _fileStorage;

    public UploadsController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

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

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest("Dozvoljeni formati: jpg, jpeg, png, webp.");
        }

        await using var stream = file.OpenReadStream();
        var path = await _fileStorage.SaveAsync(stream, extension, cancellationToken);

        return Ok(new { path });
    }
}