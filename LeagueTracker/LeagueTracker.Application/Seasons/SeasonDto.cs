

namespace LeagueTracker.Application.Seasons;

public class SeasonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
}

public class CreateSeasonDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int LeagueId { get; set; }
}
