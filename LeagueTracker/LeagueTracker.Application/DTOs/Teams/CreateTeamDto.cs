
namespace LeagueTracker.Application.DTOs.Teams;

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
