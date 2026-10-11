

using LeagueTracker.Application.DTOs.Teams;

namespace LeagueTracker.Application.Interfaces;

public interface ITeamService
{
    Task<List<TeamDto>> GetAsync(int? seasonId);
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto> CreateAsync(CreateTeamDto dto);
    Task<bool> DeleteAsync(int id);
    Task<TeamDto?> UpdateAsync(int id, CreateTeamDto dto);
    Task<bool> RegisterToSeasonAsync(int teamId, int seasonId);
    Task<bool> UnregisterFromSeasonAsync(int teamId, int seasonId);
    Task<List<TeamSeasonHistoryDto>> GetHistoryAsync(int teamId);
}
