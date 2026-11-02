using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Queries.GetTicketHistory;

public sealed record GetTicketHistoryQuery(Guid TicketId) : IRequest<Result<ICollection<TicketHistoryResponse>>>;
