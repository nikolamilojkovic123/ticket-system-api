using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Dashboard.Queries;

public sealed class GetDashboardQuery : IRequest<Result<DashboardResponse>>
{
}
