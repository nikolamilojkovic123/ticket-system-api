using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.UserManagment.Entities;

namespace TicketSystem.Infrastructure.Database.Configuration.UserManagment;

internal sealed class UserEntityConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(t => t.Id);

        builder.HasMany(x => x.AssignedTickets)
          .WithOne(x => x.Assignee)
          .HasForeignKey(x => x.AssigneeId);
    }
}
