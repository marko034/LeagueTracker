

namespace LeagueTracker.Domain.Entities;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }

    public int? LeagueId { get; set; }
    public League? League { get; set; }
}
