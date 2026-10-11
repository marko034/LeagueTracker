
namespace LeagueTracker.Application.DTOs.Games;

public class GameDto
{
    public int Id { get; set; }
    public int HomePlayer1Id { get; set; }
    public int? HomePlayer2Id { get; set; }
    public int AwayPlayer1Id { get; set; }
    public int? AwayPlayer2Id { get; set; }
    public bool HomeWon { get; set; }
    public List<CreateGameSetDto> Sets { get; set; } = new();
}
