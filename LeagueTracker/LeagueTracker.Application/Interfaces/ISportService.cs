
using LeagueTracker.Application.DTOs.Sports;

namespace LeagueTracker.Application.Interfaces;

public interface ISportService
{
    Task<List<SportDto>> GetAllAsync();
    Task<SportDto?> GetByIdAsync(int id);
    Task<SportDto> CreateAsync(CreateSportDto dto);
    Task<bool> DeleteAsync(int id);

}
