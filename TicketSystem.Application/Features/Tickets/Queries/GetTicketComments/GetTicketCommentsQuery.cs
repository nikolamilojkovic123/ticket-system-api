using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Queries.GetTicketComments;

public sealed record GetTicketCommentsQuery(Guid TicketId) : IRequest<Result<ICollection<TicketCommentResponse>>>
{
}
