namespace LeagueTracker.Domain.Entities;

public class TeamSeason
{
    public int Id { get; set; }

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public ICollection<PlayerRegistration> Players { get; set; } = new List<PlayerRegistration>();
}