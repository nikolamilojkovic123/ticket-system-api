using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Tickets.Commands.AddTicketComment;

public sealed class AddTicketCommentHandler(ITicketRepository ticketRepository)
    : IRequestHandler<AddTicketCommentCommand, Result<TicketCommentResponse>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<Result<TicketCommentResponse>> Handle(AddTicketCommentCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }
    private async Task<Result<TicketCommentResponse>> HandleInnerAsync([NotNull] AddTicketCommentCommand request, CancellationToken ct = default)
    {
        Ticket? ticket = await _ticketRepository.GetTicketByIdAsync(request.TicketId, ct);
        if (ticket == null)
        {
            return Result<TicketCommentResponse>.Fail(Error.None);
        }

        TicketComment comment = new(request.TicketId, request.Author, request.Content);

        await _ticketRepository.AddCommentAsync(comment, ct);

        return Result<TicketCommentResponse>.Success(new TicketCommentResponse
        {
            Id = comment.Id,
            Author = comment.Author,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
        });
    }

}
