using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Queries.SearchTickets;

public sealed record SearchTicketsQuery(
    string Query,
    int Page = 1,
    int PageSize = 10)
    : IRequest<Result<PagedResult<TicketResponse>>>;