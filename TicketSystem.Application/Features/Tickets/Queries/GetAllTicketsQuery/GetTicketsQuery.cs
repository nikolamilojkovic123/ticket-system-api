using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Mediator;

namespace TicketSystem.Application.Features.Tickets.Queries.GetAllTicketsQuery;

public sealed record GetTicketsQuery(int Page,
    int PageSize) : IRequest<PagedResult<TicketResponse>>
{
}
