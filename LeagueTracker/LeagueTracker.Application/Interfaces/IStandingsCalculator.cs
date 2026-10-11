


using LeagueTracker.Application.DTOs.Standings;
using LeagueTracker.Domain.Entities;

namespace LeagueTracker.Application.Interfaces;

public interface IStandingsCalculator
{
    string SportName { get; }
    List<StandingsRowDto> Calculate(List<Match> matches, List<Team> teams);
}
