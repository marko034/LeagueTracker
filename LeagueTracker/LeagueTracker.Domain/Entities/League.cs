namespace LeagueTracker.Domain.Entities;

public class League
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SportId { get; set; }
    public Sport Sport { get; set; } = null;

    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}