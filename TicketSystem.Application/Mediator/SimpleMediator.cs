using Microsoft.Extensions.DependencyInjection;

namespace TicketSystem.Application.Mediator;

public class SimpleMediator
{
    private readonly IServiceProvider _provider;

    public SimpleMediator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = _provider.GetService(handlerType)
            ?? throw new Exception($"Handler not found for {request.GetType().Name}");

        return await ((dynamic)handler).Handle((dynamic)request, ct);
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification
    {
        var handlers = _provider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers)
        {
            await handler.Handle(notification, ct);
        }
    }
}
