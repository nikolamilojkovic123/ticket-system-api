using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace TicketSystem.Infrastructure.ExternalServices
{
    public static class RabbitMqConnectionFactory
    {
        public static async Task<IConnection> CreateConnectionAsync(IConfiguration config)
        {
            ConnectionFactory factory = new()
            {
                HostName = config["RabbitMq:Host"],
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"]
            };

            return factory.CreateConnection();
        }
    }
}
