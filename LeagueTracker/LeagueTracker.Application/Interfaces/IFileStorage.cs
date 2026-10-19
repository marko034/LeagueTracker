namespace LeagueTracker.Application.Interfaces;

public record StoredFile(string RelativePath, DateTime LastWriteUtc);
public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string extension, CancellationToken cancellationToken = default);

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    IReadOnlyList<StoredFile> List();
}
