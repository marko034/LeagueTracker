

namespace LeagueTracker.Application.DTOs.Players;

public class PlayerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? TeamId { get; set; }       // popunjeno samo kad se traži u kontekstu sezone
    public string? TeamName { get; set; }
}
