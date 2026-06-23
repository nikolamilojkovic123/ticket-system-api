using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery()
    : IRequest<Result<GetUsersResponse>>;
