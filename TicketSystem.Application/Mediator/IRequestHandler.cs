using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Application.Mediator;


public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}
