using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.ConversationManagement.Entities;

namespace TicketSystem.Infrastructure.Database.Configuration.ConversationManagement;

internal sealed class ConversationEntityConfiguration
     : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        builder.HasKey(t => t.Id);

        builder.HasMany(x => x.Messages)
           .WithOne(x => x.Conversation)
           .HasForeignKey(x => x.ConversationId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Messages)
          .HasField("_messages")
          .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
