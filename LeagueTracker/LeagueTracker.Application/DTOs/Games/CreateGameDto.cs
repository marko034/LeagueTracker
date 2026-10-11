

using LeagueTracker.Domain.Entities;

namespace LeagueTracker.Application.DTOs.Games;

public class CreateGameDto
{
    public int MatchId { get; set; }
    public GameType Type { get; set; }

    public int HomePlayer1Id { get; set; }
    public int? HomePlayer2Id { get; set; }

    public int AwayPlayer1Id { get; set; }
    public int? AwayPlayer2Id { get; set; }

    public List<CreateGameSetDto> Sets { get; set; } = new();
}
