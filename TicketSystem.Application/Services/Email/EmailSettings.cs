namespace TicketSystem.Application.Services.Email;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "TicketSystem Ticket System";
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
    public string AdminEmail { get; set; } = string.Empty;
}
