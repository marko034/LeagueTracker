using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations;

public class PlayerRegistrationConfiguration : IEntityTypeConfiguration<PlayerRegistration>
{
    public void Configure(EntityTypeBuilder<PlayerRegistration> builder)
    {
        builder.HasIndex(pr => new { pr.PlayerId, pr.TeamSeasonId }).IsUnique();

        builder.HasOne(pr => pr.Player)
            .WithMany(p => p.Registrations)
            .HasForeignKey(pr => pr.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.TeamSeason)
            .WithMany(ts => ts.Players)
            .HasForeignKey(pr => pr.TeamSeasonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
