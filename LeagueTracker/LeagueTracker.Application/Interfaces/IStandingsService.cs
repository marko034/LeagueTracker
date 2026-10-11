

using LeagueTracker.Application.DTOs.Standings;

namespace LeagueTracker.Application.Interfaces;

public interface IStandingsService
{
    Task<List<StandingsRowDto>> GetStandingsAsync(int seasonId);
}
