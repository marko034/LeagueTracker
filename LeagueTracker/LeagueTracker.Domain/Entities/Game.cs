
using System.Text.Json.Serialization;

namespace LeagueTracker.Domain.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameType
{
    Single,
    Double
}

public class Game
{
    public int Id { get; set; }
    public GameType Type { get; set; }

    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int HomePlayer1Id { get; set; }
    public Player HomePlayer1 { get; set; } = null!;

    public int? HomePlayer2Id { get; set; }
    public Player? HomePlayer2 { get; set; }

    public int AwayPlayer1Id { get; set; }
    public Player AwayPlayer1 { get; set; } = null!;

    public int? AwayPlayer2Id { get; set; }
    public Player? AwayPlayer2 { get; set; } 

    public ICollection<GameSet> Sets { get; set; } = new List<GameSet>();
}
