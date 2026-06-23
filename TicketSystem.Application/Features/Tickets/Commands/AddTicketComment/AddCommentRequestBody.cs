namespace TicketSystem.Application.Features.Tickets.Commands.AddTicketComment;

public sealed class AddCommentRequestBody
{
    public string Content { get; set; } = default!;
}
