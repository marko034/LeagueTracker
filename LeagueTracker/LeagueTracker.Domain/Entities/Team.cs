namespace LeagueTracker.Domain.Entities;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}