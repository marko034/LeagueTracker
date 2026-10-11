

using LeagueTracker.Application.DTOs.Teams;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly LeagueTrackerDbContext _context;
    private readonly IStandingsService _standingsService;

    public TeamService(LeagueTrackerDbContext context, IStandingsService standingsService)
    {
        _context = context;
        _standingsService = standingsService;
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto dto)
    {
        var team = new Team
        {
            Name = dto.Name,
            LogoUrl = dto.LogoUrl
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return new TeamDto { Id = team.Id, Name = team.Name, LogoUrl = team.LogoUrl };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team is null)
        {
            return false;
        }

        if (await _context.TeamSeasons.AnyAsync(ts => ts.TeamId == id))
        {
            throw new BusinessRuleException("Tim je prijavljen u jednu ili više sezona i ne može biti obrisan.");
        }

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        return await _context.Teams
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TeamDto { Id = t.Id, Name = t.Name, LogoUrl = t.LogoUrl })
            .FirstOrDefaultAsync();
    }

    public async Task<List<TeamDto>> GetAsync(int? seasonId)
    {
        var query = _context.Teams
            .AsNoTracking()
            .AsQueryable();

        if (seasonId.HasValue)
        {
            var sId = seasonId.Value;
            query = query.Where(t => t.TeamSeasons.Any(ts => ts.SeasonId == sId));
        }

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new TeamDto { Id = t.Id, LogoUrl = t.LogoUrl, Name = t.Name })
            .ToListAsync();
    }

    public async Task<List<TeamSeasonHistoryDto>> GetHistoryAsync(int teamId)
    {
        var seasons = await _context.TeamSeasons
            .AsNoTracking()
            .Where(ts => ts.TeamId == teamId)
            .OrderByDescending(ts => ts.Season.StartDate)
            .Select(ts => new
            {
                ts.SeasonId,
                SeasonName = ts.Season.Name,
                LeagueName = ts.Season.League.Name
            })
            .ToListAsync();

        var history = new List<TeamSeasonHistoryDto>();

        foreach (var season in seasons)
        {
            var table = await _standingsService.GetStandingsAsync(season.SeasonId);
            var index = table.FindIndex(r => r.TeamId == teamId);
            var row = index >= 0 ? table[index] : null;

            history.Add(new TeamSeasonHistoryDto
            {
                SeasonId = season.SeasonId,
                SeasonName = season.SeasonName,
                LeagueName = season.LeagueName,
                Position = index >= 0 ? index + 1 : null,
                Played = row?.Played ?? 0,
                Wins = row?.Wins ?? 0,
                Draws = row?.Draws ?? 0,
                Losses = row?.Losses ?? 0,
                Points = row?.Points ?? 0
            });
        }

        return history;
    }

    public async Task<bool> RegisterToSeasonAsync(int teamId, int seasonId)
    {
        if (!await _context.Teams.AnyAsync(t => t.Id == teamId) ||
            !await _context.Seasons.AnyAsync(s => s.Id == seasonId))
        {
            return false;
        }

        if (await _context.TeamSeasons.AnyAsync(ts => ts.TeamId == teamId && ts.SeasonId == seasonId))
        {
            throw new BusinessRuleException("Tim je već prijavljen na ovu sezonu.");
        }

        _context.TeamSeasons.Add(new TeamSeason { TeamId = teamId, SeasonId = seasonId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnregisterFromSeasonAsync(int teamId, int seasonId)
    {
        var teamSeason = await _context.TeamSeasons
            .FirstOrDefaultAsync(ts => ts.TeamId == teamId && ts.SeasonId == seasonId);
        if (teamSeason is null) return false;

        if (await _context.PlayerRegistrations.AnyAsync(r => r.TeamSeasonId == teamSeason.Id))
        {
            throw new BusinessRuleException("Tim ima prijavljene igrače u ovoj sezoni. Prvo ih odjavite.");
        }

        if (await _context.Matches.AnyAsync(m => m.SeasonId == seasonId &&
                                                 (m.HomeTeamId == teamId || m.AwayTeamId == teamId)))
        {
            throw new BusinessRuleException("Tim ima mečeve u ovoj sezoni i ne može biti odjavljen.");
        }

        _context.TeamSeasons.Remove(teamSeason);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TeamDto?> UpdateAsync(int id, CreateTeamDto dto)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team is null)
        {
            return null;
        }

        team.Name = dto.Name;
        team.LogoUrl = dto.LogoUrl;

        await _context.SaveChangesAsync();

        return new TeamDto { Id = team.Id, Name = team.Name, LogoUrl = team.LogoUrl };
    }
}
