namespace LeagueTracker.Application.DTOs.Teams;

public class TeamSeasonHistoryDto
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string LeagueName { get; set; } = string.Empty;
    public int? Position { get; set; }
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }
}