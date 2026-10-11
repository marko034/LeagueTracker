
using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations;

public class GameSetConfiguration : IEntityTypeConfiguration<GameSet>
{
    public void Configure(EntityTypeBuilder<GameSet> builder)
    {
        builder.HasOne(gs => gs.Game)
            .WithMany(g => g.Sets)
            .HasForeignKey(gs => gs.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
