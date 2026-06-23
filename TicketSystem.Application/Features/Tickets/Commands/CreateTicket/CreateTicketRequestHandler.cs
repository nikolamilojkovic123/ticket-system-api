using Microsoft.Extensions.Options;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.Email;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketRequestHandler(
    ITicketRepository ticketRepository,
    IEventBus eventBus,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings)
    : IRequestHandler<CreateTicketRequest, Guid>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

    public Task<Guid> Handle(CreateTicketRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Guid> HandleInnerAsync([NotNull] CreateTicketRequest request, CancellationToken ct = default)
    {
        Ticket t = new(
                request.Title,
                request.Description,
                request.Category,
                request.Priority
            );

        if (request.UserId.HasValue)
            t.AssignTo(request.UserId.Value);

        await _ticketRepository.CreateTicketAsync(t, ct);

        string adminEmail = emailSettings.Value.AdminEmail;
        if (!string.IsNullOrWhiteSpace(adminEmail))
        {
            await emailService.SendTicketCreatedAsync(t.Id, t.Title, adminEmail, ct);
        }

        await _eventBus.PublishAsync(new TicketCreatedEvent
        {
            TicketId = t.Id
        }, ct);

        return t.Id;
    }
}
