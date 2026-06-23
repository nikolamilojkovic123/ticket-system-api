namespace TicketSystem.Application.Features.Tickets.Dtos;

public sealed class TicketCommentResponse
{
    public Guid Id { get; set; }
    public string Author { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
