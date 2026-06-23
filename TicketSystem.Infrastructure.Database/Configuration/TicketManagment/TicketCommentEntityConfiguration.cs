using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.TicketManagment.Entities;

namespace TicketSystem.Infrastructure.Database.Configuration.TicketManagment;

internal sealed class TicketCommentEntityConfiguration
    : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("TicketComments");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Author).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Content).IsRequired();
    }
}
