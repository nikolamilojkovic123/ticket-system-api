namespace TicketSystem.Application.Services.Admin;

public sealed class AdminSettings
{
    public const string SectionName = "AdminSettings";

    public string AdminEmail { get; set; } = string.Empty;
}
