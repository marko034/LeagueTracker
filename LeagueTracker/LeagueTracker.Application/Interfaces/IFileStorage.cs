namespace LeagueTracker.Application.Interfaces;

public record StoredFile(string RelativePath, DateTime LastWriteUtc);
public interface IFileStorage
{
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);

    Task DeleteAsync(string? path, CancellationToken cancellationToken = default);

    IReadOnlyList<StoredFile> List();
}
