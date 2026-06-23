using Microsoft.Extensions.Logging;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Application.Services.AI;

namespace TicketSystem.Infrastructure.Messaging.Consumers;

public class TicketCreatedConsumer
{
    private readonly ITicketAiService _ticketAi;
    private readonly ILogger<TicketCreatedConsumer> _logger;

    public TicketCreatedConsumer(
        ITicketAiService ticketAi,
        ILogger<TicketCreatedConsumer> logger)
    {
        _ticketAi = ticketAi;
        _logger = logger;
    }

    public async Task Handle(TicketCreatedEvent message)
    {
        try
        {
            await _ticketAi.EnrichTicketAsync(message.TicketId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI enrichment failed for ticket {TicketId}", message.TicketId);
        }
    }
}
