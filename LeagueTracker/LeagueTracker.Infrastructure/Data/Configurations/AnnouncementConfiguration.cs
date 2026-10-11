

using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Content).IsRequired().HasMaxLength(4000);

        builder.HasOne(a=>a.League)
            .WithMany()
            .HasForeignKey(a => a.LeagueId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
