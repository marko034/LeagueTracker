

using LeagueTracker.Application.DTOs.Games;

namespace LeagueTracker.Application.Interfaces;

public interface IGameService
{
    Task<GameDto> RecordGameAsync(CreateGameDto dto, string userId, bool isAdmin);
    Task<FinalizeMatchResultDto> FinalizeMatchAsync(int matchId, string userId, bool isAdmin);
    Task<bool> VerifyMatchAsync(int matchId);

}
