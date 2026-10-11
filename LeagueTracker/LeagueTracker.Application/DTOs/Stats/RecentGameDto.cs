
namespace LeagueTracker.Application.DTOs.Stats;

public class RecentGameDto
{
    public DateTime Date { get; set; }
    public string OpponentTeamName { get; set; } = string.Empty;
    public bool Won { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
}
