using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TicketSystem.Infrastructure.Notifications.Hubs;

[Authorize]
public sealed class TicketHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userId}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User-{userId}");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinTicketGroup(string ticketId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"Ticket-{ticketId}");

    public async Task LeaveTicketGroup(string ticketId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ticket-{ticketId}");
}
