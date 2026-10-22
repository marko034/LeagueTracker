

using LeagueTracker.Application.Common;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using NetVips;

namespace LeagueTracker.Infrastructure.Services;

public class ImageService : IImageService
{
    private const int MaxSide = 10_000;
    private const long MaxPixels = 40_000_000;
    private readonly IFileStorage _fileStorage;

    public ImageService(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<string> ProcessAndSaveAsync(Stream upload, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await upload.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        var header = bytes.AsSpan(0, Math.Min(bytes.Length, 16));
        if(ImageSignature.Detect(header) == DetectedImageType.Unknown)
        {
            throw new BusinessRuleException("Fajl nije ispravna slika. Dozvoljeni su JPG, PNG i WebP.");
        }

        var (width, height) = ReadSizes(bytes);
        if (width > MaxSide || height > MaxSide || (long)width * height > MaxPixels)
        {
            throw new BusinessRuleException("Slika je prevelikih dimenzija.");
        }

        var id = Guid.NewGuid();

        var fullPath = await SaveAsync($"{id}.webp", Render(bytes, ImageVariants.FullSize), cancellationToken);

        try
        {
            await SaveAsync(
                $"{id}{ImageVariants.SmallSuffix}.webp", Render(bytes, ImageVariants.SmallSize), cancellationToken);
        }
        catch
        {
            await _fileStorage.DeleteAsync(fullPath);
            throw;
        }

        return fullPath;
    }

    private async Task<string> SaveAsync(string fileName, byte[] data, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream(data);
        return await _fileStorage.SaveAsync(fileName, stream, cancellationToken);
    }

    private static byte[] Render(byte[] bytes, int maxSize)
    {
        try
        {
            using var thumbnail = Image.ThumbnailBuffer(bytes, maxSize, height: maxSize, size: Enums.Size.Down);
            return thumbnail.WriteToBuffer(".webp[Q=82,strip]");
        }
        catch
        {
            throw new BusinessRuleException("Slika je oštećena ili nije podržana.");
        }
    }

    private static (int width, int height) ReadSizes(byte[] bytes)
    {
        try
        {
            using var image = Image.NewFromBuffer(bytes);
            return (image.Width, image.Height);
        }
        catch(VipsException)
        {
            throw new BusinessRuleException("Slika je oštećena ili nije podržana.");
        }
    }


}
