using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.TicketManagment.Entities;


namespace TicketSystem.Infrastructure.Database.Configuration.TicketManagment;

internal sealed class TicketEntityConfiguration
    : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(cc => cc.Id);

        builder.HasOne(t => t.Assignee)
             .WithMany(u => u.AssignedTickets)
             .HasForeignKey(t => t.AssigneeId)
             .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Comments)
             .WithOne(c => c.Ticket)
             .HasForeignKey(c => c.TicketId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(t => t.Comments)
             .HasField("_comments")
             .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
