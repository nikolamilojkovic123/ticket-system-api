namespace TicketSystem.Application.Features.Tickets.Dtos;

public sealed class TicketHistoryResponse
{
    public Guid Id { get; set; }
    public string Field { get; set; } = default!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangedBy { get; set; } = default!;
    public DateTime ChangedAt { get; set; }
}
