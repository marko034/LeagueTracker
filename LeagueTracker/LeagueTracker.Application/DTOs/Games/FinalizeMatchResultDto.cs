

namespace LeagueTracker.Application.DTOs.Games;

public class FinalizeMatchResultDto
{
    public int MatchId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Status { get; set; } = string.Empty;
}
