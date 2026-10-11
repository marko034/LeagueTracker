
using LeagueTracker.Application.DTOs.Announcement;

namespace LeagueTracker.Application.Interfaces;

public interface IAnnouncementService
{
    Task<List<AnnouncementDto>> GetRecentAsync(int count = 20);
    Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto);
    Task<bool> DeleteAsync(int id);
}
