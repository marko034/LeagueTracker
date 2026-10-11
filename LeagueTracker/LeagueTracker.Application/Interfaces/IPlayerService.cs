
using LeagueTracker.Application.DTOs.Players;

namespace LeagueTracker.Application.Interfaces;

public interface IPlayerService
{
    Task<List<PlayerDto>> GetAsync(int? teamId, int? seasonId);
    Task<PlayerDto?> GetByIdAsync(int id, int? seasonId);
    Task<PlayerDto> CreateAsync(CreatePlayerDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PlayerDto?> UpdateAsync(int id, CreatePlayerDto dto);
    Task<bool> RegisterAsync(int playerId, RegisterPlayerDto dto);
    Task<bool> UnregisterAsync(int playerId, int seasonId);

}
