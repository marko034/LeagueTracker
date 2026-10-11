namespace LeagueTracker.Domain.Entities;

public class PlayerRegistration
{
    public int Id { get; set; }

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int TeamSeasonId { get; set; }
    public TeamSeason TeamSeason { get; set; } = null!;
}