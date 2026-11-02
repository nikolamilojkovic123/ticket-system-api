using TicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public sealed class UpdateTicketCommand : IRequest<Result<UpdateTicketResponse>>
{
    public Guid Id { get; init; }
    public CreateTicketRequest Data { get; init; }
    public string? ChangedBy { get; init; }

    public UpdateTicketCommand(Guid id, CreateTicketRequest data, string? changedBy = null)
    {
        Id = id;
        Data = data;
        ChangedBy = changedBy;
    }
}
