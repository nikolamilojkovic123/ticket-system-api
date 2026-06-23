namespace TicketSystem.Application.Features.Users.Queries.GetUserStats;

public sealed class UserStatsDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Total { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ClosedCount { get; set; }
    public double? AvgResolutionDays { get; set; }
}
