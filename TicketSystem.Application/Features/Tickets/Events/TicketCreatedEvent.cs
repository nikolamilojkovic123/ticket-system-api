namespace TicketSystem.Application.Features.Tickets.Events;

public sealed class TicketCreatedEvent
{
    public Guid TicketId { get; set; }
}
