using RabbitMQ.Client;
using TicketSystem.Application.Services;
using System.Text;
using System.Text.Json;

namespace TicketSystem.Infrastructure.ExternalServices;

public class RabbitMqEventBus : IEventBus
{
    private readonly IConnection _connection;

    public RabbitMqEventBus(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default)
    {
        using IModel channel = _connection.CreateModel();

        var queueName = typeof(T).Name;

        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }
}
