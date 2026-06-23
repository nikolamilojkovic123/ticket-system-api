using Microsoft.AspNetCore.SignalR;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Infrastructure.Notifications.Hubs;

namespace TicketSystem.Infrastructure.Notifications;

internal sealed class NotificationService(IHubContext<TicketHub> hubContext) : INotificationService
{
    public Task NotifyTicketAssignedAsync(Guid ticketId, string ticketTitle, Guid assigneeId, CancellationToken ct = default)
        => hubContext.Clients
            .Group($"User-{assigneeId}")
            .SendAsync("TicketAssigned", ticketId.ToString(), ticketTitle, ct);

    public Task NotifyTicketUpdatedAsync(Guid ticketId, string ticketTitle, Guid assigneeId, CancellationToken ct = default)
        => hubContext.Clients
            .Group($"User-{assigneeId}")
            .SendAsync("TicketUpdated", ticketId.ToString(), ticketTitle, ct);

    public Task NotifyCommentAddedAsync(long caseId, CommentDto comment, CancellationToken ct = default)
        => hubContext.Clients
            .Group($"Ticket-{caseId}")
            .SendAsync("CommentAdded", caseId, comment, ct);

    public Task NotifyCommentDeletedAsync(long caseId, int commentId, CancellationToken ct = default)
        => hubContext.Clients
            .Group($"Ticket-{caseId}")
            .SendAsync("CommentDeleted", caseId, commentId, ct);
}
