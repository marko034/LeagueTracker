using LeagueTracker.Application.DTOs.Standings;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services.Standings;

public class StandingsService : IStandingsService
{
    private readonly LeagueTrackerDbContext _context;
    private readonly IStandingsCalculatorFactory _standingsCalculatorFactory;

    public StandingsService(LeagueTrackerDbContext context, IStandingsCalculatorFactory factory)
    {
        _context = context;
        _standingsCalculatorFactory = factory;
    }
    public async Task<List<StandingsRowDto>> GetStandingsAsync(int seasonId)
    {
        var season = await _context.Seasons
            .AsNoTracking()
            .Include(s => s.League)
            .ThenInclude(l => l.Sport)
            .FirstOrDefaultAsync(s => s.Id == seasonId);

        if(season is null)
        {
            return new List<StandingsRowDto>();
        }

        var teams = await _context.TeamSeasons
            .AsNoTracking()
            .Where(ts => ts.SeasonId == seasonId)
            .Select(ts => ts.Team)
            .ToListAsync();

        var matches = await _context.Matches
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId)
            .ToListAsync();

        var calculator = _standingsCalculatorFactory.GetCalculator(season.League.Sport.Name);
        return calculator.Calculate(matches, teams);
    }
}
