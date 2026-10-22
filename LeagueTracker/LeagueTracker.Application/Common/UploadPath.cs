
using LeagueTracker.Application.Exceptions;
using System.Text.RegularExpressions;

namespace LeagueTracker.Application.Common;

public static class UploadPath
{
    private static readonly Regex Pattern = 
        new (@"^/uploads/[A-Za-z0-9\-]{1,64}\.(jpg|jpeg|png|webp)$", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex StoredFilePattern =
        new(@"^/uploads/[A-Za-z0-9\-]{1,64}(_sm)?\.(jpg|jpeg|png|webp)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool IsValid(string path) => path is not null && Pattern.IsMatch(path);

    public static bool IsStoredFile(string? path) => path is not null && StoredFilePattern.IsMatch(path);

    public static string? Normalize(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var trimmed = path.Trim();

        if (!IsValid(trimmed))
        {
            throw new BusinessRuleException("Neispravna putanja slike. Slika mora biti otpremljena kroz aplikaciju.");
        }

        return trimmed;
    }
}
