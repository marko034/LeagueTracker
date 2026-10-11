
using LeagueTracker.Application.DTOs.Stats;
using LeagueTracker.Domain.Entities;

namespace LeagueTracker.Application.Interfaces;

public interface IPlayerStatsCalculator
{
    string SportName { get; }
    PlayerStatsTableDto Calculate(List<Player> players, List<Match> matches);
}
