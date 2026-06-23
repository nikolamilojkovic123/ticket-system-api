using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Mediator;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Application.Features.Tickets.Queries.GetFilteredTickets;

public sealed record GetFilteredTicketsQuery(
    int Page,
    int PageSize,
    ICollection<TicketStatus> Statuses,
    ICollection<TicketPriority> Priorities,
    ICollection<TicketCategory> Categories,
    Guid? AssigneeId,
    DateTime? DateFrom,
    DateTime? DateTo
) : IRequest<PagedResult<TicketResponse>>;
