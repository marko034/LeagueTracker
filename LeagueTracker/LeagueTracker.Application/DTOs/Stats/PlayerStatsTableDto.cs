
namespace LeagueTracker.Application.DTOs.Stats;

public class PlayerStatsTableDto
{
    public List<StatColumnDto> Columns { get; set; } = new ();
    public List<PlayerStatsRowDto> Rows { get; set; } = new();
}
