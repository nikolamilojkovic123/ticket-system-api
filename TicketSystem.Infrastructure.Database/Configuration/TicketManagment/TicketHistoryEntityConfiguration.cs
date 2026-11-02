using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.TicketManagment.Entities;

namespace TicketSystem.Infrastructure.Database.Configuration.TicketManagment;

internal sealed class TicketHistoryEntityConfiguration
    : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("TicketHistory");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.Field).IsRequired().HasMaxLength(32);
        builder.Property(h => h.ChangedBy).IsRequired().HasMaxLength(256);
        builder.Property(h => h.OldValue);
        builder.Property(h => h.NewValue);

        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(h => h.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => new { h.TicketId, h.ChangedAt });
    }
}
