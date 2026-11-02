using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.Email;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketRequestHandler(
    ITicketRepository ticketRepository,
    IUserRepository userRepository,
    IEventBus eventBus,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings)
    : IRequestHandler<CreateTicketRequest, Result<CreateTicketResponse>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

    public Task<Result<CreateTicketResponse>> Handle(CreateTicketRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<CreateTicketResponse>> HandleInnerAsync([NotNull] CreateTicketRequest request, CancellationToken ct = default)
    {
        Ticket t = new(
                request.Title,
                request.Description,
                request.Category,
                request.Priority,
                request.CreatedBy
            );

        if (request.UserId.HasValue)
        {
            User? assignee = await userRepository.GetUserByIdAsync(request.UserId.Value, ct);
            t.AssignTo(request.UserId.Value, assignee?.FullName, request.CreatedBy);
        }

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

        return Result<CreateTicketResponse>.Success(new CreateTicketResponse { TicketId = t.Id });
    }
}
