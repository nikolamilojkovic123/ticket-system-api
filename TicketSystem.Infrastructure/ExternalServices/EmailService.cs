using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TicketSystem.Application.Services.Email;

namespace TicketSystem.Infrastructure.ExternalServices;

internal sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task SendTicketCreatedAsync(Guid ticketId, string ticketTitle, string recipientEmail, CancellationToken ct = default)
    {
        string subject = $"New ticket created: {ticketTitle}";
        string body = $"""
            <h2>New Ticket Created</h2>
            <p><strong>Title:</strong> {HtmlEncode(ticketTitle)}</p>
            <p><strong>Ticket ID:</strong> {ticketId}</p>
            <p>The ticket has been created and AI analysis is in progress.</p>
            """;

        return SendEmailAsync(recipientEmail, subject, body, ct);
    }

    public Task SendTicketAssignedAsync(Guid ticketId, string ticketTitle, string assigneeEmail, CancellationToken ct = default)
    {
        string subject = $"Ticket assigned to you: {ticketTitle}";
        string body = $"""
            <h2>Ticket Assigned</h2>
            <p><strong>Title:</strong> {HtmlEncode(ticketTitle)}</p>
            <p><strong>Ticket ID:</strong> {ticketId}</p>
            <p>This ticket has been assigned to you.</p>
            """;

        return SendEmailAsync(assigneeEmail, subject, body, ct);
    }

    public Task SendTicketStatusChangedAsync(Guid ticketId, string ticketTitle, string newStatus, string recipientEmail, CancellationToken ct = default)
    {
        string subject = $"Ticket status changed: {ticketTitle}";
        string body = $"""
            <h2>Ticket Status Updated</h2>
            <p><strong>Title:</strong> {HtmlEncode(ticketTitle)}</p>
            <p><strong>Ticket ID:</strong> {ticketId}</p>
            <p><strong>New Status:</strong> {HtmlEncode(newStatus)}</p>
            """;

        return SendEmailAsync(recipientEmail, subject, body, ct);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost) || string.IsNullOrWhiteSpace(_settings.SenderEmail))
        {
            _logger.LogWarning("Email not configured — skipping send to {Email}", toEmail);
            return;
        }

        MimeMessage message = new();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        BodyBuilder builder = new() { HtmlBody = htmlBody };
        message.Body = builder.ToMessageBody();

        try
        {
            using SmtpClient client = new();

            SecureSocketOptions socketOptions = _settings.SmtpPort == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, socketOptions, ct);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}: {Subject}", toEmail, subject);
        }
    }

    private static string HtmlEncode(string value) => System.Net.WebUtility.HtmlEncode(value);
}
