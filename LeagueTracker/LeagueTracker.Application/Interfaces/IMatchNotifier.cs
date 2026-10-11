

namespace LeagueTracker.Application.Interfaces;

public interface IMatchNotifier
{
    Task NotifyMatchUpdatedAsync(int matchId, int seasonId);
}
