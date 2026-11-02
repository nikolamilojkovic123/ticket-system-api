using System.Diagnostics.CodeAnalysis;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler(
    ITicketRepository ticketRepository,
    INotificationService notificationService
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

        entity.ChangeStatus(request.Status, request.ChangedBy);

        bool historyChanged = entity.PendingHistory.Count > 0;

        await ticketRepository.UpdateAsync(entity, ct);

        if (historyChanged)
            await notificationService.NotifyTicketHistoryUpdatedAsync(entity.Id, ct);

        return Result<UpdateTicketStatusResponse>.Success(
            new UpdateTicketStatusResponse()
            {
                StatusChanged = true
            });
    }

}
