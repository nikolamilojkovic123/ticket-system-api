namespace TicketSystem.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyTicketAssignedAsync(Guid ticketId, string ticketTitle, Guid assigneeId, CancellationToken ct = default);
    Task NotifyTicketUpdatedAsync(Guid ticketId, string ticketTitle, Guid assigneeId, CancellationToken ct = default);
    Task NotifyCommentAddedAsync(long caseId, CommentDto comment, CancellationToken ct = default);
    Task NotifyCommentDeletedAsync(long caseId, int commentId, CancellationToken ct = default);
}
