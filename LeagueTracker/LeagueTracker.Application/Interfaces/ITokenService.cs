

namespace LeagueTracker.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(string userId, string email, string displayName, IList<string> roles);
}
