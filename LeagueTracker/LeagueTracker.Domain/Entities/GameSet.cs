
namespace LeagueTracker.Domain.Entities;

public class GameSet
{
    public int Id { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null;
}
