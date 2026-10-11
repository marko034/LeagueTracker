namespace LeagueTracker.Domain.Entities;

public enum MatchStatus
{
    Scheduled,
    InProgress,
    AwaitingVerification,
    Disputed,
    Verified
}

public class Match
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;

    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;

    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}