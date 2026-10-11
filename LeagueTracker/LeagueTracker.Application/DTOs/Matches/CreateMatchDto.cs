

namespace LeagueTracker.Application.DTOs.Matches;

public class CreateMatchDto
{
    public int SeasonId { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public DateTime ScheduledAt { get; set; }
}
