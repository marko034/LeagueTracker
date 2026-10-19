
using LeagueTracker.Application.Common;
using LeagueTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeagueTracker.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
{
    private const string RequestPatch = "/uploads";

    private readonly string _folder;
    private readonly ILogger<LocalFileStorage> _logger;

    public LocalFileStorage(IWebHostEnvironment environment, ILogger<LocalFileStorage> logger)
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        _folder = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(_folder);
        _logger = logger;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!UploadPath.IsValid(path))
        {
            return Task.CompletedTask;
        }

        try
        {
            File.Delete(Path.Combine(_folder, Path.GetFileName(path!)));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Brisanje fajla {Path} nije uspelo.", path);
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<StoredFile> List() =>
        new DirectoryInfo(_folder)
        .EnumerateFiles()
        .Select(f => new StoredFile($"{RequestPatch}/{f.Name}", f.LastWriteTimeUtc))
        .ToList();

    public async Task<string> SaveAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        var fileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        await using var target = new FileStream(
            Path.Combine(_folder, fileName), FileMode.CreateNew, FileAccess.Write);
        await content.CopyToAsync(target,cancellationToken);
        return $"{RequestPatch}/{fileName}";
    }
}
