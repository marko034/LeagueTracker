

using LeagueTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LeagueTracker.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public int? PlayerId { get; set; }
    public Player? Player { get; set; }
}
