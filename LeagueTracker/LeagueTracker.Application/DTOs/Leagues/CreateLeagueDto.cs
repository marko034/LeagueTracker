

namespace LeagueTracker.Application.DTOs.Leagues;

public class CreateLeagueDto
{
    public string Name { get; set; } = string.Empty;
    public int SportId { get; set; }
}
