

using LeagueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeagueTracker.Infrastructure.Data.Configurations
{
    internal class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(EntityTypeBuilder<League> builder)
        {
            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(l=>l.Sport)
                .WithMany(s => s.Leagues)
                .HasForeignKey(l => l.SportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
