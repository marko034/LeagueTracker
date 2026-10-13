

using LeagueTracker.Application.DTOs.Matches;

namespace LeagueTracker.Application.Interfaces;

public interface IMatchService
{
    Task<List<MatchDto>> GetBySeasonAsync(int seasonId);
    Task<MatchDto?> GetByIdAsync(int id);
    Task<MatchDto> CreateAsync(CreateMatchDto matchDto);
    Task<List<MatchDto>> GetAwaitingVerificationAsync();
    Task<bool> DeleteAsync(int id);
    Task<bool> ResetResultAsync(int id);
}
