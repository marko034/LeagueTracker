
using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations;

public class TeamSeasonConfiguration : IEntityTypeConfiguration<TeamSeason>
{
    public void Configure(EntityTypeBuilder<TeamSeason> builder)
    {
        builder.HasIndex(ts => new { ts.TeamId, ts.SeasonId }).IsUnique();

        builder.HasOne(ts => ts.Team)
            .WithMany(ts => ts.TeamSeasons)
            .HasForeignKey(ts => ts.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ts => ts.Season)
            .WithMany()
            .HasForeignKey(ts => ts.SeasonId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}
