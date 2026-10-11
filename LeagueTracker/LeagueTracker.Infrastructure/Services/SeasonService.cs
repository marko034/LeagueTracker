

using LeagueTracker.Application.Interfaces;
using LeagueTracker.Application.Seasons;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class SeasonService : ISeasonService
{
    private readonly LeagueTrackerDbContext _context;

    public SeasonService(LeagueTrackerDbContext context)
    {
        _context = context;
    }
    public async Task<SeasonDto> CreateAsync(CreateSeasonDto dto)
    {
        var season = new Season
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            LeagueId = dto.LeagueId
        };

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();

        return new SeasonDto
        {
            Id = season.Id,
            Name = season.Name,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            LeagueId = season.LeagueId,
            LeagueName = (await _context.Leagues.FindAsync(season.LeagueId))!.Name
        };
    }

    public async Task<List<SeasonDto>> GetAllAsync()
    {
        return await _context.Seasons
            .AsNoTracking()
            .OrderByDescending(s => s.StartDate)
            .Select(s => new SeasonDto
            {
                Id = s.Id,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                LeagueId = s.LeagueId,
                LeagueName = s.League.Name
            })
            .ToListAsync();
    }
}
