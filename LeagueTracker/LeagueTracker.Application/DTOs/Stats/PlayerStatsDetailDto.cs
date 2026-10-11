

namespace LeagueTracker.Application.DTOs.Stats;

public class PlayerStatsDetailDto
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
    public List<RecentGameDto> RecentForm { get; set; } = new();
    public List<SeasonStatsDto> SeasonBreakdown { get; set; } = new();
}


public class SeasonStatsDto
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string LeagueName { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int SetsWon { get; set; }
    public int SetsLost { get; set; }
}