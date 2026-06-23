using TicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public sealed class UpdateTicketCommand : IRequest<Result<bool>>
{
    public Guid Id { get; init; }
    public CreateTicketRequest Data { get; init; }

    public UpdateTicketCommand(Guid id, CreateTicketRequest data)
    {
        Id = id;
        Data = data;
    }
}
