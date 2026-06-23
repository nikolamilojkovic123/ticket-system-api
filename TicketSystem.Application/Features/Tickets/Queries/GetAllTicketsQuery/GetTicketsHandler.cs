using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Mediator;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Tickets.Queries.GetAllTicketsQuery;

public sealed class GetTicketsHandler(ITicketRepository ticketRepository) :
    IRequestHandler<GetTicketsQuery, PagedResult<TicketResponse>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    public Task<PagedResult<TicketResponse>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<PagedResult<TicketResponse>> HandleInnerAsync([NotNull] GetTicketsQuery request, CancellationToken ct = default)
    {
        (ICollection<Ticket>? tickets, int totalCount) = await _ticketRepository.GetPagedAsync(request.Page, request.PageSize, ct);

        return new PagedResult<TicketResponse>
        {
            Items = tickets.Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UserId = t.AssigneeId,
                Name = t.Assignee != null ? t.Assignee.FullName : string.Empty
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

}
