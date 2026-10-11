using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeagueTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UploadsController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("image")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("Fajl je prazan");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Dozvoljeni formati su: .jpg, .jpeg, .png, .webp");
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest("Fajl je prevelik (max 5 MB).");
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new { url });
    }
}
