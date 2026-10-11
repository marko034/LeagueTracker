
using LeagueTracker.Application.DTOs.Announcement;
using LeagueTracker.Application.Interfaces;
using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly LeagueTrackerDbContext _context;

    public AnnouncementService(LeagueTrackerDbContext context)
    {
        _context = context;
    }
    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto)
    {
        var announcement = new Announcement
        {
            Title = dto.Title,
            Content = dto.Content,
            LeagueId = dto.LeagueId,
            PublishedAt = DateTime.UtcNow
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return new AnnouncementDto
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            PublishedAt = announcement.PublishedAt,
            LeagueName = dto.LeagueId.HasValue ? (await _context.Leagues.FindAsync(dto.LeagueId))?.Name : null
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return false;
        }

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AnnouncementDto>> GetRecentAsync(int count = 20)
    {
        return await _context.Announcements
            .AsNoTracking()
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                PublishedAt = a.PublishedAt,
                LeagueName = a.League != null ? a.League.Name : null
            })
            .ToListAsync();
    }
}
