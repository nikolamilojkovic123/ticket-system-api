using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TicketSystem.Application.Features.Tickets.Events;
using TicketSystem.Infrastructure.Messaging.Consumers;
using System.Text;
using System.Text.Json;

namespace TicketSystem.Infrastructure.Messaging.Listeners;

public class TicketCreatedListener : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;

    public TicketCreatedListener(
        IConnection connection,
        IServiceProvider serviceProvider)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IModel channel = _connection.CreateModel();

        string queue = nameof(TicketCreatedEvent);

        channel.QueueDeclare(queue, true, false, false);

        EventingBasicConsumer consumer = new(channel);

        consumer.Received += async (sender, e) =>
        {
            byte[] body = e.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);

            TicketCreatedEvent? message = JsonSerializer.Deserialize<TicketCreatedEvent>(json);

            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                TicketCreatedConsumer handler = scope.ServiceProvider.GetRequiredService<TicketCreatedConsumer>();

                await handler.Handle(message!);
            }
            catch (Exception ex)
            {
                int retryCount = 0;

                if (e.BasicProperties.Headers != null &&
                    e.BasicProperties.Headers.TryGetValue("retry-count", out object? value))
                {
                    retryCount = Convert.ToInt32(value);
                }

                if (retryCount < 3)
                {
                    IBasicProperties props = channel.CreateBasicProperties();
                    props.Headers = new Dictionary<string, object>
                {
                    { "retry-count", retryCount + 1 }
                };

                    await Task.Delay(2000);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: nameof(TicketCreatedEvent),
                        basicProperties: props,
                        body: body);
                }
                else
                {
                    Console.WriteLine("💀 Message failed permanently");
                }
            }
        };

        channel.BasicConsume(queue, true, consumer);

        return Task.CompletedTask;
    }
}
