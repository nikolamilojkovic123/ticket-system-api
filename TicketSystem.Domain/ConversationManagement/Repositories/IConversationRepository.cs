using TicketSystem.Domain.ConversationManagement.Entities;

namespace TicketSystem.Domain.ConversationManagement.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetConversationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Conversation conversation, CancellationToken ct);
    Task UpdateAsync(Conversation conversation, CancellationToken ct);
}
