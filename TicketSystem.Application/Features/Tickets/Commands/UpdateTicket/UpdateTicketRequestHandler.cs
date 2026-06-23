using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.Email;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public sealed class UpdateTicketRequestHandler(
    ITicketRepository ticketRepository,
    IEventBus eventBus,
    INotificationService notificationService,
    IEmailService emailService)
    : IRequestHandler<UpdateTicketCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<bool>> HandleInnerAsync([NotNull] UpdateTicketCommand request, CancellationToken ct = default)
    {
        Ticket? entity = await ticketRepository.GetTicketByIdAsync(request.Id, ct);
        if (entity is null)
            return Result<bool>.Fail(Error.None);

        Guid? previousAssigneeId = entity.AssigneeId;
        var previousStatus = entity.Status;

        entity.Update(request.Data.Title, request.Data.Description);
        entity.ChangePriority(request.Data.Priority);
        entity.AssignTo(request.Data.UserId);
        entity.ChangeStatus(request.Data.Status);

        await ticketRepository.UpdateAsync(entity, ct);
        await eventBus.PublishAsync(new TicketCreatedEvent { TicketId = entity.Id }, ct);

        bool assigneeChanged = false;

        if (request.Data.UserId.HasValue)
        {
            Guid assigneeId = request.Data.UserId.Value;

            if (previousAssigneeId != assigneeId)
            {
                assigneeChanged = true;
                await notificationService.NotifyTicketAssignedAsync(entity.Id, entity.Title, assigneeId, ct);

                if (entity.Assignee is not null)
                {
                    await emailService.SendTicketAssignedAsync(
                        entity.Id, entity.Title, entity.Assignee.Email, ct);
                }
            }
            else
            {
                await notificationService.NotifyTicketUpdatedAsync(entity.Id, entity.Title, assigneeId, ct);
            }
        }

        if (!assigneeChanged && previousStatus != request.Data.Status && entity.Assignee is not null)
        {
            await emailService.SendTicketStatusChangedAsync(
                entity.Id, entity.Title, request.Data.Status.ToString(), entity.Assignee.Email, ct);
        }

        return Result<bool>.Success(true);
    }
}
