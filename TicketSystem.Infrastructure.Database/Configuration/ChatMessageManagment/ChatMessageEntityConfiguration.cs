using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.ChatMessageManagement.Entities;

namespace TicketSystem.Infrastructure.Database.Configuration.ChatMessageManagment
{
    internal class ChatMessageEntityConfiguration
          : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Content).IsRequired();

            builder.HasIndex(x => x.ConversationId);
        }
    }
}
