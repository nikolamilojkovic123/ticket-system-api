namespace TicketSystem.Application.Services.AI;

public interface ITicketAiService
{
    Task<bool?> EnrichTicketAsync(Guid ticketId, CancellationToken ct = default);
}
