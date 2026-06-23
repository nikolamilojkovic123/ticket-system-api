using TicketSystem.Application.Mediator;
using TicketSystem.Domain.TicketManagment.Enums;
namespace TicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketRequest : IRequest<Guid>
{
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public TicketCategory Category { get; init; }
    public TicketPriority Priority { get; init; }
    public TicketStatus Status { get; init; }
    public Guid? UserId { get; set; }
}