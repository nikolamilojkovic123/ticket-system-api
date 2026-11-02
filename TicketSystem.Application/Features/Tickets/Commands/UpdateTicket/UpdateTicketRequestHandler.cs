using System.Diagnostics.CodeAnalysis;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.Email;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Enums;
using TicketSystem.Domain.TicketManagment.Repositories;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicket;

public sealed class UpdateTicketRequestHandler(
    ITicketRepository ticketRepository,
    IUserRepository userRepository,
    IEventBus eventBus,
    INotificationService notificationService,
    IEmailService emailService)
    : IRequestHandler<UpdateTicketCommand, Result<UpdateTicketResponse>>
{
    public Task<Result<UpdateTicketResponse>> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<UpdateTicketResponse>> HandleInnerAsync([NotNull] UpdateTicketCommand request, CancellationToken ct = default)
    {
        Ticket? entity = await ticketRepository.GetTicketByIdAsync(request.Id, ct);
        if (entity is null)
            return Result<UpdateTicketResponse>.Fail(ErrorData.None);

        Guid? previousAssigneeId = entity.AssigneeId;
        TicketStatus previousStatus = entity.Status;

        string? assigneeName = null;
        if (request.Data.UserId.HasValue && request.Data.UserId != previousAssigneeId)
        {
            User? assignee = await userRepository.GetUserByIdAsync(request.Data.UserId.Value, ct);
            assigneeName = assignee?.FullName;
        }

        entity.Update(request.Data.Title, request.Data.Description, request.ChangedBy);
        entity.ChangePriority(request.Data.Priority, request.ChangedBy);
        entity.AssignTo(request.Data.UserId, assigneeName, request.ChangedBy);
        entity.ChangeStatus(request.Data.Status, request.ChangedBy);

        bool historyChanged = entity.PendingHistory.Count > 0;

        await ticketRepository.UpdateAsync(entity, ct);

        if (historyChanged)
            await notificationService.NotifyTicketHistoryUpdatedAsync(entity.Id, ct);
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

        return Result<UpdateTicketResponse>.Success(new()
        {
            IsSuccess = true
        });
    }
}
