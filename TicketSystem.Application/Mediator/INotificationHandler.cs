using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Application.Mediator;

public interface INotificationHandler<TNotification>
where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken ct);
}
