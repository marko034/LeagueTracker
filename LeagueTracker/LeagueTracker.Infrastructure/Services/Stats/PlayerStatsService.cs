
using LeagueTracker.Application.DTOs.Stats;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services.Stats;

public class PlayerStatsService : IPlayerStatsService
{
    private readonly LeagueTrackerDbContext _context;
    private readonly IPlayerStatsCalculatorFactory _statsCalculatorFactory;

    public PlayerStatsService(LeagueTrackerDbContext context, IPlayerStatsCalculatorFactory factory)
    {
        _context = context;
        _statsCalculatorFactory = factory;
    }

    public async Task<PlayerStatsDetailDto> GetPlayerDetailAsync(int playerId, int? seasonId)
    {
        var matches = await _context.Matches
            .AsNoTracking()
            .Include(m => m.Season).ThenInclude(s => s.League)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Games).ThenInclude(g => g.Sets)
            .Where(m => m.Status == MatchStatus.Verified &&
                    m.Games.Any(g => g.HomePlayer1Id == playerId || g.HomePlayer2Id == playerId ||
                                     g.AwayPlayer1Id == playerId || g.AwayPlayer2Id == playerId))
            .OrderByDescending(m => m.ScheduledAt)
            .ToListAsync();

        var result = new PlayerStatsDetailDto();
        var breakdown = new Dictionary<int, SeasonStatsDto>();

        foreach (var match in matches)
        {
            if (!breakdown.TryGetValue(match.SeasonId, out var seasonStats))
            {
                seasonStats = new SeasonStatsDto
                {
                    SeasonId = match.SeasonId,
                    SeasonName = match.Season.Name,
                    LeagueName = match.Season.League.Name
                };
                breakdown[match.SeasonId] = seasonStats;
            }

            var inScope = seasonId is null || match.SeasonId == seasonId;

            var playerGames = match.Games.Where(g => g.HomePlayer1Id ==playerId || g.HomePlayer2Id ==playerId
            || g.AwayPlayer1Id == playerId || g.AwayPlayer2Id == playerId);

            foreach (var game in playerGames)
            {
                var isHome = game.HomePlayer1Id == playerId || game.HomePlayer2Id == playerId;

                var homeSets = game.Sets.Count(g => g.HomeScore > g.AwayScore);
                var awaySets = game.Sets.Count(g => g.HomeScore < g.AwayScore);

                var playerSetsWon = isHome ? homeSets : awaySets;
                var playerSetsLost = isHome ? awaySets : homeSets;

                var won = playerSetsWon > playerSetsLost;

                seasonStats.SetsWon += playerSetsWon;
                seasonStats.SetsLost += playerSetsLost;
                if (won) seasonStats.Wins++; else seasonStats.Losses++;

                if (!inScope) continue;

                result.SetsWon += playerSetsWon;
                result.SetsLost += playerSetsLost;
                if (won)
                    result.Wins++;
                else
                    result.Losses++;

                if(result.RecentForm.Count < 5)
                {
                    result.RecentForm.Add(new RecentGameDto
                    {
                        Date = match.ScheduledAt,
                        OpponentTeamName = isHome ? match.AwayTeam.Name : match.HomeTeam.Name,
                        Won = won,
                        SetsWon = playerSetsWon,
                        SetsLost = playerSetsLost
                    });
                }
            }
        }
        result.SeasonBreakdown = breakdown.Values.OrderByDescending(s => s.SeasonId).ToList();
        return result;
    }

    public async Task<PlayerStatsTableDto> GetStatsAsync(int seasonId)
    {
        var season = await _context.Seasons
            .AsNoTracking()
            .Include(s => s.League)
            .ThenInclude(l => l.Sport)
            .FirstOrDefaultAsync(s => s.Id == seasonId);

        if(season is null)
        {
            return new PlayerStatsTableDto();
        }

        var players = await _context.PlayerRegistrations
            .AsNoTracking()
            .Where(r => r.TeamSeason.SeasonId == seasonId)
            .Select(r => r.Player)
            .ToListAsync();

        var matches = await _context.Matches
            .AsNoTracking()
            .Include(m => m.Games)
            .ThenInclude(g => g.Sets)
            .Where(m => m.SeasonId == seasonId)
            .ToListAsync();

        var calculator = _statsCalculatorFactory.GetCalculator(season.League.Sport.Name);
        return calculator.Calculate(players, matches);
    }
}
