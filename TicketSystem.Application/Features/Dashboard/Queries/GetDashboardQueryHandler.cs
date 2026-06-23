using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.DasboardManagement;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public sealed class GetDashboardQueryHandler(IDashboardRepository dashboardRepository) :
    IRequestHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    private readonly IDashboardRepository _repo = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));

    public async Task<Result<DashboardResponse>> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var stats = await _repo.GetStatsAsync(ct);
        return new DashboardResponse
        {
            TotalTickets      = stats.Total,
            OpenTickets       = stats.Open,
            InProgressTickets = stats.InProgress,
            ResolvedTickets   = stats.Resolved,
            AverageSeverity   = stats.AverageSeverity,
            CriticalCount     = stats.CriticalCount,
        };
    }
}
