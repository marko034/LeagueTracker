using LeagueTracker.Application.DTOs.Standings;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;

namespace LeagueTracker.Infrastructure.Services.Standings;

public class TableTennisStandingsCalculator : IStandingsCalculator
{
    public string SportName => "Stoni tenis";

    public List<StandingsRowDto> Calculate(List<Match> matches, List<Team> teams)
    {
        var rows = teams.ToDictionary(t => t.Id, t => new StandingsRowDto
        {
            TeamId = t.Id,
            TeamName = t.Name,
            TeamLogoUrl = t.LogoUrl
        });

        foreach (var match in matches.Where(m => m.Status == MatchStatus.Verified))
        {
            if(match.HomeScore is null || match.AwayScore is null)
            {
                continue;
            }

            var home = rows[match.HomeTeamId];
            var away = rows[match.AwayTeamId];

            home.Played++;
            away.Played++;

            if (match.HomeScore > match.AwayScore)
            {
                home.Wins++;
                home.Points += 3;
                away.Losses++;
            }
            else if(match.HomeScore < match.AwayScore)
            {
                home.Losses++;
                away.Points += 3;
                away.Wins++;
            }
            else
            {
                home.Draws++;
                home.Points++;
                away.Points++;
                away.Draws++;
            }
        }

        return rows.Values.OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Wins)
                .ThenBy(r => r.Played)
                .ToList();
    }
}
