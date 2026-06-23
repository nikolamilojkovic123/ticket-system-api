using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.DasboardManagement;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Infrastructure.Database.Repositories;

public sealed class DashboardRepository(ApplicationDbContext context) : IDashboardRepository
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<DashboardStats> GetStatsAsync(CancellationToken ct)
    {
        var counts = await _context.Tickets
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total      = g.Count(),
                Open       = g.Count(x => x.Status == TicketStatus.Open),
                InProgress = g.Count(x => x.Status == TicketStatus.InProgress),
                Resolved   = g.Count(x => x.Status == TicketStatus.Closed),
                Critical   = g.Count(x => x.SeverityScore > 0.70),
                AvgSeverity = g.Average(x => (double?)x.SeverityScore) ?? 0
            })
            .FirstOrDefaultAsync(ct);

        return counts is null
            ? new DashboardStats()
            : new DashboardStats
            {
                Total          = counts.Total,
                Open           = counts.Open,
                InProgress     = counts.InProgress,
                Resolved       = counts.Resolved,
                CriticalCount  = counts.Critical,
                AverageSeverity = Math.Round(counts.AvgSeverity * 100, 1)
            };
    }
}
