using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;

public sealed class TicketResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
}
