
namespace LeagueTracker.Application.DTOs.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? HomeTeamLogoUrl { get; set; }
    public string? AwayTeamLogoUrl { get; set; }
    public int SeasonId { get; set; }
}
