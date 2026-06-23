using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Tickets.Commands.AddTicketComment;

public sealed record AddTicketCommentCommand(Guid TicketId, string Author, string Content)
    : IRequest<Result<TicketCommentResponse>>
{
}
