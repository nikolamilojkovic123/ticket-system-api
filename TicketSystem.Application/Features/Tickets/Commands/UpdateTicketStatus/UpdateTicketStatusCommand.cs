using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicketStatus;

public sealed record UpdateTicketStatusCommand(Guid id, TicketStatus Status, string? ChangedBy = null)
    : IRequest<Result<UpdateTicketStatusResponse>>;
