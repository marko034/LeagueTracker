namespace LeagueTracker.Application.Interfaces;

public interface IImageService
{
    Task<string> ProcessAndSaveAsync(Stream upload, CancellationToken cancellationToken = default);
}