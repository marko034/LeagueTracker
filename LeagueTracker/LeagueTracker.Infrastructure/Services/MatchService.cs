

using LeagueTracker.Application.DTOs.Matches;
using LeagueTracker.Application.Exceptions;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class MatchService : IMatchService
{
    public LeagueTrackerDbContext _context;

    public MatchService(LeagueTrackerDbContext context)
    {
        _context = context;
    }
    public async Task<MatchDto> CreateAsync(CreateMatchDto matchDto)
    {

        if (matchDto.HomeTeamId == matchDto.AwayTeamId)
        {
            throw new BusinessRuleException("Domaćin i gost ne mogu biti isti tim.");
        }

        var registered = await _context.TeamSeasons.CountAsync(ts =>
            ts.SeasonId == matchDto.SeasonId && (ts.TeamId == matchDto.HomeTeamId || ts.TeamId == matchDto.AwayTeamId));

        if (registered != 2)
        {
            throw new BusinessRuleException("Oba tima moraju biti prijavljena na izabranu sezonu.");
        }

        var match = new Match
        {
            SeasonId = matchDto.SeasonId,
            HomeTeamId = matchDto.HomeTeamId,
            AwayTeamId = matchDto.AwayTeamId,
            ScheduledAt = matchDto.ScheduledAt,
            Status = MatchStatus.Scheduled
        };

        _context.Matches.Add(match);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(match.Id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var match = await _context.Matches.FindAsync(id);
        if (match is null) return false;

        _context.Matches.Remove(match);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MatchDto>> GetAwaitingVerificationAsync()
    {
        return await _context.Matches
            .AsNoTracking()
            .Where(x => x.Status == MatchStatus.AwaitingVerification)
            .OrderBy(x => x.ScheduledAt)
            .Select(x => new MatchDto
            {
                HomeTeamId = x.HomeTeamId,
                AwayTeamId = x.AwayTeamId,
                AwayTeamName = x.AwayTeam.Name,
                HomeTeamName = x.HomeTeam.Name,
                HomeScore = x.HomeScore,
                AwayScore = x.AwayScore,
                Status = x.Status.ToString(),
                ScheduledAt = x.ScheduledAt,
                Id = x.Id,
                HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                SeasonId = x.SeasonId
            }).ToListAsync();
    }

    public async Task<MatchDto?> GetByIdAsync(int id)
    {
        return await _context.Matches
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                HomeTeamId = m.HomeTeamId,
                HomeTeamName = m.HomeTeam.Name,
                AwayTeamId = m.AwayTeamId,
                AwayTeamName = m.AwayTeam.Name,
                ScheduledAt = m.ScheduledAt,
                Status = m.Status.ToString(),
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore,
                HomeTeamLogoUrl = m.HomeTeam.LogoUrl,
                AwayTeamLogoUrl = m.AwayTeam.LogoUrl,
                SeasonId = m.SeasonId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<MatchDto>> GetBySeasonAsync(int seasonId)
    {
        return await _context.Matches
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId)
            .OrderBy(m => m.ScheduledAt)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                ScheduledAt = m.ScheduledAt,
                Status = m.Status.ToString(),
                HomeTeamId = m.HomeTeamId,
                HomeTeamName= m.HomeTeam.Name,
                AwayTeamId = m.AwayTeamId,
                AwayTeamName = m.AwayTeam.Name,
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore,
                HomeTeamLogoUrl= m.HomeTeam.LogoUrl,
                AwayTeamLogoUrl= m.AwayTeam.LogoUrl,
                SeasonId = m.SeasonId
            })
            .ToListAsync();
    }
}
