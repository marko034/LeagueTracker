
using LeagueTracker.Application.DTOs.Stats;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;

namespace LeagueTracker.Infrastructure.Services.Stats;

public class TableTennisPlayerStatsCalculator : IPlayerStatsCalculator
{
    public string SportName => "Stoni tenis";
    public PlayerStatsTableDto Calculate(List<Player> players, List<Match> matches)
    {
        var table = new PlayerStatsTableDto
        {
            Columns = new List<StatColumnDto>
            {
                new(){ Key = "wins", Label ="Pobede" },
                new(){ Key = "losses", Label = "Porazi" },
                new() { Key = "setsWon", Label = "Osvojeni setovi" },
                new() { Key = "setsLost", Label = "Izgubljeni setovi" }
            }
        };

        var statsByPlayer = players.ToDictionary(p => p.Id, p => new Dictionary<string, int>
        {
            ["wins"] = 0,
            ["losses"] = 0,
            ["setsWon"] = 0,
            ["setsLost"] = 0
        });

        var verifiedMatches = matches.Where(m => m.Status == MatchStatus.Verified);

        foreach (var match in verifiedMatches)
        {
            foreach(var game in match.Games)
            {
                var homeSetsWon = game.Sets.Count(s => s.HomeScore > s.AwayScore);
                var awaySetsWon = game.Sets.Count(s => s.HomeScore < s.AwayScore);
                var homeWonGame = homeSetsWon > awaySetsWon;

                var homePlayers = new[] { game.HomePlayer1Id, game.HomePlayer2Id }
                .Where(id => id.HasValue)
                .Select(id => id!.Value);

                var awayPlayers = new[] { game.AwayPlayer1Id, game.AwayPlayer2Id }
                .Where(id => id.HasValue)
                .Select(id => id!.Value);

                foreach (var playerId in homePlayers)
                {
                    if(!statsByPlayer.TryGetValue(playerId, out var stats))
                    {
                        continue;
                    }

                    stats["setsWon"] += homeSetsWon;
                    stats["setsLost"] += awaySetsWon;
                    stats[homeWonGame ? "wins" : "losses"]++;
                }

                foreach (var playerId in awayPlayers)
                {
                    if (!statsByPlayer.TryGetValue(playerId, out var stats))
                    {
                        continue;
                    }

                    stats["setsWon"] += awaySetsWon;
                    stats["setsLost"] += homeSetsWon;
                    stats[homeWonGame ? "losses" : "wins"]++;
                }
            }
        }

        foreach (var player in players)
        {
            table.Rows.Add(new PlayerStatsRowDto
            {
                PlayerId = player.Id,
                PlayerName = $"{player.FirstName} {player.LastName}",
                Values = statsByPlayer[player.Id]
            });
        }

        table.Rows = table.Rows
        .OrderByDescending(r => r.Values["wins"])
        .ThenByDescending(r => r.Values["setsWon"] - r.Values["setsLost"])
        .ToList();

        return table;
    }
}
