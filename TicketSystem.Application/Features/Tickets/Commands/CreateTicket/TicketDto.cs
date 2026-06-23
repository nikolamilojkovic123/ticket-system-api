using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public sealed class TicketDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TicketCategory Category { get; init; }
    public TicketPriority Priority { get; init; }
    public TicketStatus Status { get; init; }
    public string? AiSummary { get; set; }
    public List<string> Keywords { get; set; } = [];
    public double SeverityScore { get; set; }
    public double Confidence { get; set; }
}
