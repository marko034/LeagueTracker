
namespace LeagueTracker.Application.DTOs.Stats;

public class PlayerStatsRowDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public Dictionary<string, int> Values { get; set; } = new();
}
