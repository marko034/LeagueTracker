namespace LeagueTracker.Domain.Entities;

public class Player
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public ICollection<PlayerRegistration> Registrations { get; set; } = new List<PlayerRegistration>();
}