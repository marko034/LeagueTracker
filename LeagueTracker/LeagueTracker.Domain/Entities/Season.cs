namespace LeagueTracker.Domain.Entities;

public class Season
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int LeagueId { get; set; }
    public League League { get; set; } = null;
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}