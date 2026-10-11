
using LeagueTracker.Application.DTOs.Leagues;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LeagueTracker.Infrastructure.Services;

public class LeagueService : ILeagueService
{
    private readonly LeagueTrackerDbContext _context;

    public LeagueService(LeagueTrackerDbContext context)
    { 
        _context = context;
    }

    public async Task<LeagueDto> CreateAsync(CreateLeagueDto dto)
    {   
        var league = new League
        {
            Name = dto.Name,
            SportId = dto.SportId
        };

        _context.Leagues.Add(league);

        await _context.SaveChangesAsync();

        return new LeagueDto 
        { 
            Id = league.Id, 
            Name = league.Name, 
            SportName = (await _context.Sports.FindAsync(dto.SportId))!.Name 
        };
    }

    public async Task<List<LeagueDto>> GetAllAsync()
    {
        return await _context.Leagues
            .AsNoTracking()
            .Select(l => new LeagueDto { Id = l.Id, Name = l.Name, SportName = l.Sport.Name })
            .ToListAsync();
    }
}
