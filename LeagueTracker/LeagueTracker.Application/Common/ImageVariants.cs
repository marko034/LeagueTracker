namespace LeagueTracker.Application.Common;

public static class ImageVariants
{
    public const int FullSize = 512;   // najduža strana osnovne slike
    public const int SmallSize = 96;   // najduža strana male slike (liste, tabele)
    public const string SmallSuffix = "_sm";

    private static readonly string[] Extensions = [".webp", ".jpg", ".jpeg", ".png"];

    public static string AssetId(string pathOrFileName)
    {
        var name = Path.GetFileNameWithoutExtension(pathOrFileName);

        return name.EndsWith(SmallSuffix, StringComparison.Ordinal)
            ? name[..^SmallSuffix.Length]
            : name;
    }

    public static IEnumerable<string> PossibleFileNames(string assetId)
    {
        foreach (var suffix in new[] { string.Empty, SmallSuffix })
        {
            foreach (var extension in Extensions)
            {
                yield return $"{assetId}{suffix}{extension}";
            }
        }
    }
}