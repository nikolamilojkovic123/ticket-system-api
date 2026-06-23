using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Queries.GetUserStats;

public sealed record GetUserStatsQuery : IRequest<Result<ICollection<UserStatsDto>>>;
