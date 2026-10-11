
using LeagueTracker.Application.DTOs.Stats;

namespace LeagueTracker.Application.Interfaces;

public interface IPlayerStatsService
{
    Task<PlayerStatsTableDto> GetStatsAsync(int seasonId);
    Task<PlayerStatsDetailDto> GetPlayerDetailAsync(int playerId, int? seasonId);
}
