
using LeagueTracker.Application.Common;
using LeagueTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeagueTracker.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
{
    private const string RequestPath = "/uploads";

    private readonly string _folder;
    private readonly ILogger<LocalFileStorage> _logger;

    public LocalFileStorage(IWebHostEnvironment environment, ILogger<LocalFileStorage> logger)
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        _folder = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(_folder);
        _logger = logger;
    }

    public IReadOnlyList<StoredFile> List() =>
        new DirectoryInfo(_folder)
        .EnumerateFiles()
        .Select(f => new StoredFile($"{RequestPath}/{f.Name}", f.LastWriteTimeUtc))
        .ToList();

    public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var relativePath = $"{RequestPath}/{fileName}";

        if (!UploadPath.IsStoredFile(relativePath))
        {
            throw new ArgumentException("Neispravno ime fajla.", nameof(fileName));
        }

        await using var target = new FileStream(
            Path.Combine(_folder, fileName), FileMode.CreateNew, FileAccess.Write);
        await content.CopyToAsync(target, cancellationToken);

        return relativePath;
    }

    public Task DeleteAsync(string? path, CancellationToken cancellationToken = default)
    {
        if (!UploadPath.IsStoredFile(path))
        {
            return Task.CompletedTask;
        }

        foreach (var fileName in ImageVariants.PossibleFileNames(ImageVariants.AssetId(path!)))
        {
            try
            {
                File.Delete(Path.Combine(_folder, fileName)); // ne baca grešku ako fajla nema
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Brisanje fajla {File} nije uspelo.", fileName);
            }
        }

        return Task.CompletedTask;
    }
}
