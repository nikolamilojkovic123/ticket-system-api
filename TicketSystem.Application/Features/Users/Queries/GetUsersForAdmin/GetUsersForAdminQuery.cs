using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Queries.GetUsersForAdmin;

public sealed record GetUsersForAdminQuery()
    : IRequest<Result<GetUsersForAdminResponse>>;
