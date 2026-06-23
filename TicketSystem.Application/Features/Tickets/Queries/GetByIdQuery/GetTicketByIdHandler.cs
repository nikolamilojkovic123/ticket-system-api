using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;

public sealed class GetTicketByIdHandler(ITicketRepository ticketRepository)
    : IRequestHandler<GetTicketByIdQuery, Result<TicketResponse>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<Result<TicketResponse>> Handle(GetTicketByIdQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<TicketResponse>> HandleInnerAsync(GetTicketByIdQuery request, CancellationToken ct)
    {
        Ticket? entity = await _ticketRepository.GetTicketByIdAsync(request.Id, ct);
        if (entity == null)
        {
            return Result<TicketResponse>.Fail(Error.None);
        }

        return Result<TicketResponse>.Success(new TicketResponse()
        {
            Id = entity.Id,
            Title = entity.Title,
            Category = entity.Category,
            Priority = entity.Priority,
            Description = entity.Description,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UserId = entity.AssigneeId,
        });
    }
}
