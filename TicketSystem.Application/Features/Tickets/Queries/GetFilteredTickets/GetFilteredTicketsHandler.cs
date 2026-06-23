using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Mediator;
using TicketSystem.Domain.TicketManagment.Models;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Tickets.Queries.GetFilteredTickets;

public sealed class GetFilteredTicketsHandler(ITicketRepository ticketRepository)
    : IRequestHandler<GetFilteredTicketsQuery, PagedResult<TicketResponse>>
{
    private readonly ITicketRepository _repo = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<PagedResult<TicketResponse>> Handle(GetFilteredTicketsQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandlerInnerAsync(request, ct);
    }

    private async Task<PagedResult<TicketResponse>> HandlerInnerAsync([NotNull] GetFilteredTicketsQuery request, CancellationToken ct)
    {
        TicketFilterParams filter = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Statuses = request.Statuses,
            Priorities = request.Priorities,
            Categories = request.Categories,
            AssigneeId = request.AssigneeId,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
        };

        (ICollection<Domain.TicketManagment.Entities.Ticket> tickets, int totalCount) = await _repo.GetFilteredAsync(filter, ct);

        return new PagedResult<TicketResponse>
        {
            Items = tickets.Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UserId = t.AssigneeId,
                Name = t.Assignee?.FullName ?? string.Empty,
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}