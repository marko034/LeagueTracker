

using LeagueTracker.Application.Seasons;

namespace LeagueTracker.Application.Interfaces;

public interface ISeasonService
{
    Task<List<SeasonDto>> GetAllAsync();
    Task<SeasonDto> CreateAsync(CreateSeasonDto dto);
}
