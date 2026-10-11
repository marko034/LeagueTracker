using LeagueTracker.Domain.Entities;
using LeagueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeagueTracker.Infrastructure.Data;

public class LeagueTrackerDbContext : IdentityDbContext<ApplicationUser>
{
    public LeagueTrackerDbContext(DbContextOptions<LeagueTrackerDbContext> options)
        : base(options)
    { 
    }

    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameSet> GameSets => Set<GameSet>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<TeamSeason> TeamSeasons => Set<TeamSeason>();
    public DbSet<PlayerRegistration> PlayerRegistrations => Set<PlayerRegistration>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeagueTrackerDbContext).Assembly);
    }

}
