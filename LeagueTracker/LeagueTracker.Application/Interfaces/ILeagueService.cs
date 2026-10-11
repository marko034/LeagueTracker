
using LeagueTracker.Application.DTOs.Leagues;

namespace LeagueTracker.Application.Interfaces;

public interface ILeagueService
{
    Task<List<LeagueDto>> GetAllAsync();
    Task<LeagueDto> CreateAsync(CreateLeagueDto dto);
}
