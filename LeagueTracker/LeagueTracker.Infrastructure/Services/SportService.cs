
using LeagueTracker.Application.DTOs.Sports;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class SportService : ISportService
{
    private readonly LeagueTrackerDbContext _context;

    public SportService(LeagueTrackerDbContext context)
    {
        _context = context;
    }
    public async Task<SportDto> CreateAsync(CreateSportDto dto)
    {
        var sport = new Sport { Name = dto.Name };
        _context.Sports.Add(sport);
        await _context.SaveChangesAsync();
        return new SportDto { Id = sport.Id, Name = sport.Name };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sport = await _context.Sports.FindAsync(id);
        if (sport is null)
        {
            return false;
        }

        _context.Sports.Remove(sport);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SportDto>> GetAllAsync()
    {
        return await _context.Sports
            .AsNoTracking()
            .Select(s => new SportDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<SportDto?> GetByIdAsync(int id)
    {
        return await _context
            .Sports
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s=> new SportDto { Id = s.Id, Name = s.Name })
            .FirstOrDefaultAsync();
    }
}
