namespace LeagueTracker.Application.Common;

public enum DetectedImageType
{
    Unknown,
    Jpeg,
    Png,
    WebP
}

public static class ImageSignature
{
    // Prepoznaje tip po prvim bajtovima, ne po ekstenziji koju je poslao klijent
    public static DetectedImageType Detect(ReadOnlySpan<byte> header)
    {
        // JPEG: FF D8 FF
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return DetectedImageType.Jpeg;
        }

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        ReadOnlySpan<byte> png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        if (header.Length >= png.Length && header[..png.Length].SequenceEqual(png))
        {
            return DetectedImageType.Png;
        }

        // WebP: "RIFF" + 4 bajta dužine + "WEBP"
        if (header.Length >= 12 &&
            header[..4].SequenceEqual("RIFF"u8) &&
            header.Slice(8, 4).SequenceEqual("WEBP"u8))
        {
            return DetectedImageType.WebP;
        }

        return DetectedImageType.Unknown;
    }
}