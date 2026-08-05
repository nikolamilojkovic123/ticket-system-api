using System.Diagnostics.CodeAnalysis;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(
    ITicketRepository ticketRepository
    )
    : IRequestHandler<UpdateTicketStatusCommand, Result<UpdateTicketStatusResponse>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    public Task<Result<UpdateTicketStatusResponse>> Handle(UpdateTicketStatusCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }
    private async Task<Result<UpdateTicketStatusResponse>> HandleInnerAsync([NotNull] UpdateTicketStatusCommand request, CancellationToken ct = default)
    {
        Ticket? entity = await _ticketRepository.GetTicketByIdAsync(request.id, ct);

        if (entity is null)
        {
            return Result<UpdateTicketStatusResponse>.Fail(ErrorData.None);
        }

        entity.ChangeStatus(request.Status);
        await ticketRepository.UpdateAsync(entity, ct);

        return Result<UpdateTicketStatusResponse>.Success(
            new UpdateTicketStatusResponse()
            {
                StatusChanged = true
            });
    }

}
