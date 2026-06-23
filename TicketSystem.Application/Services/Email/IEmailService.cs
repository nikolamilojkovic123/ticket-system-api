namespace TicketSystem.Application.Services.Email;

public interface IEmailService
{
    Task SendTicketCreatedAsync(Guid ticketId, string ticketTitle, string recipientEmail, CancellationToken ct = default);
    Task SendTicketAssignedAsync(Guid ticketId, string ticketTitle, string assigneeEmail, CancellationToken ct = default);
    Task SendTicketStatusChangedAsync(Guid ticketId, string ticketTitle, string newStatus, string recipientEmail, CancellationToken ct = default);
}
