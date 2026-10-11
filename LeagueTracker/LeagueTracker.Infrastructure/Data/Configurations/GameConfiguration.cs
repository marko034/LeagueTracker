
using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasOne(g=>g.Match)
            .WithMany(m => m.Games)
            .HasForeignKey(g => g.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.HomePlayer1)
            .WithMany()
            .HasForeignKey(g => g.HomePlayer1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.HomePlayer2)
            .WithMany()
            .HasForeignKey(g => g.HomePlayer2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.AwayPlayer1)
            .WithMany()
            .HasForeignKey(g => g.AwayPlayer1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.AwayPlayer2)
            .WithMany()
            .HasForeignKey(g => g.AwayPlayer2Id)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
