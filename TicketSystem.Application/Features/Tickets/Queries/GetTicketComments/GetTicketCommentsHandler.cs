using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Queries.GetTicketComments;

public sealed class GetTicketCommentsHandler(ITicketRepository ticketRepository)
    : IRequestHandler<GetTicketCommentsQuery, Result<ICollection<TicketCommentResponse>>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<Result<ICollection<TicketCommentResponse>>> Handle(GetTicketCommentsQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandlerInnerAsync(request, ct);
    }

    private async Task<Result<ICollection<TicketCommentResponse>>> HandlerInnerAsync(GetTicketCommentsQuery request, CancellationToken ct)
    {
        ICollection<TicketComment> comments = await _ticketRepository.GetCommentsAsync(request.TicketId, ct);

        ICollection<TicketCommentResponse> response = comments.Select(c => new TicketCommentResponse
        {
            Id = c.Id,
            Author = c.Author,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
        }).ToList();

        return Result<ICollection<TicketCommentResponse>>.Success(response);
    }
}
