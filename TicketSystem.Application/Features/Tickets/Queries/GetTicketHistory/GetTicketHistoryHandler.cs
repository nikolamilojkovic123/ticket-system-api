using TicketSystem.Application.Common;
using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Queries.GetTicketHistory;

public sealed class GetTicketHistoryHandler(ITicketRepository ticketRepository)
    : IRequestHandler<GetTicketHistoryQuery, Result<ICollection<TicketHistoryResponse>>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<Result<ICollection<TicketHistoryResponse>>> Handle(GetTicketHistoryQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<ICollection<TicketHistoryResponse>>> HandleInnerAsync(GetTicketHistoryQuery request, CancellationToken ct)
    {
        if (!await _ticketRepository.ExistsAsync(request.TicketId, ct))
            return Result<ICollection<TicketHistoryResponse>>.Fail(ApplicationErrors.NotFound);

        List<TicketHistory> history = await _ticketRepository.GetHistoryAsync(request.TicketId, ct);

        ICollection<TicketHistoryResponse> response = history.Select(h => new TicketHistoryResponse
        {
            Id = h.Id,
            Field = h.Field,
            OldValue = h.OldValue,
            NewValue = h.NewValue,
            ChangedBy = h.ChangedBy,
            ChangedAt = DateTime.SpecifyKind(h.ChangedAt, DateTimeKind.Utc),
        }).ToList();

        return Result<ICollection<TicketHistoryResponse>>.Success(response);
    }
}
