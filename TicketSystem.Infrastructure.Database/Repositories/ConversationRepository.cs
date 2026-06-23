using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.ConversationManagement.Entities;
using TicketSystem.Domain.ConversationManagement.Repositories;

namespace TicketSystem.Infrastructure.Database.Repositories;

public sealed class ConversationRepository(ApplicationDbContext dbContext) : IConversationRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Conversation?> GetConversationByIdAsync(Guid id, CancellationToken cancellationToken)
    =>
        await _dbContext.Conversations.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Conversation conversation, CancellationToken ct)
    {
        await _dbContext.Conversations.AddAsync(conversation, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken ct)
    {
        _dbContext.Conversations.Update(conversation);
        await _dbContext.SaveChangesAsync(ct);
    }

}
