

namespace LeagueTracker.Application.DTOs.Players;

public class CreatePlayerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
