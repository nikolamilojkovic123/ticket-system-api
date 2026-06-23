namespace TicketSystem.Domain.UserManagment;

public sealed class UserStats
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Total { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ClosedCount { get; set; }
    public double? AvgResolutionDays { get; set; }
}
