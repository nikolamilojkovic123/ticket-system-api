using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;

public sealed record GetTicketByIdQuery(Guid Id) : IRequest<Result<TicketResponse>>
{

}
