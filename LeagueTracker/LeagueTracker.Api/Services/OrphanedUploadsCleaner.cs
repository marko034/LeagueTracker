using LeagueTracker.Application.Common;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Api.Services;

public class OrphanedUploadsCleaner : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);//TimeSpan.FromHours(6);

    // Slika se otprema pre nego što se zapis sačuva, pa mlađe fajlove ne diramo
    private static readonly TimeSpan GracePeriod = TimeSpan.FromMinutes(1);// TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFileStorage _storage;
    private readonly ILogger<OrphanedUploadsCleaner> _logger;

    public OrphanedUploadsCleaner(
        IServiceScopeFactory scopeFactory,
        IFileStorage storage,
        ILogger<OrphanedUploadsCleaner> logger)
    {
        _scopeFactory = scopeFactory;
        _storage = storage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(StartupDelay, stoppingToken);

            using var timer = new PeriodicTimer(Interval);
            do
            {
                try
                {
                    await CleanAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Čišćenje neiskorišćenih slika nije uspelo.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // aplikacija se gasi
        }
    }

    private async Task CleanAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LeagueTrackerDbContext>();

        var logos = await context.Teams
            .Where(t => t.LogoUrl != null)
            .Select(t => t.LogoUrl!)
            .ToListAsync(cancellationToken);

        var photos = await context.Players
            .Where(p => p.PhotoUrl != null)
            .Select(p => p.PhotoUrl!)
            .ToListAsync(cancellationToken);

        var inUse = logos.Concat(photos)
            .Select(ImageVariants.AssetId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var cutoff = DateTime.UtcNow - GracePeriod;

        var orphanedAssets = _storage.List()
            .GroupBy(file => ImageVariants.AssetId(file.RelativePath), StringComparer.OrdinalIgnoreCase)
            .Where(group => !inUse.Contains(group.Key) && group.Max(file => file.LastWriteUtc) < cutoff)
            .ToList();

        foreach (var asset in orphanedAssets)
        {
            _logger.LogInformation("Brišem neiskorišćenu sliku {AssetId}", asset.Key);
            await _storage.DeleteAsync(asset.First().RelativePath, cancellationToken);
        }

        if (orphanedAssets.Count > 0)
        {
            _logger.LogInformation("Obrisano {Count} neiskorišćenih slika.", orphanedAssets.Count);
        }
    }
}