namespace TicketSystem.Domain.DasboardManagement;

public interface IDashboardRepository
{
    Task<DashboardStats> GetStatsAsync(CancellationToken ct);
}

public sealed class DashboardStats
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public double AverageSeverity { get; set; }
    public int CriticalCount { get; set; }
}
