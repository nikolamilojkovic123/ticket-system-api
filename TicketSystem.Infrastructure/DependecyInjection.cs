using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.Admin;
using TicketSystem.Application.Services.AI;
using TicketSystem.Application.Services.Email;
using TicketSystem.Application.Services.Ui;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Infrastructure.ExternalServices;
using TicketSystem.Infrastructure.Messaging.Consumers;
using TicketSystem.Infrastructure.Messaging.Listeners;
using TicketSystem.Infrastructure.Notifications;

namespace TicketSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IOpenAiService, OpenAiService>();
        services.AddScoped<IRagService, OllamaRagService>();
        services.AddScoped<ITicketAiService, TicketAiService>();
        services.AddScoped<ITextToSpeechService, TextToSpeechService>();
        services.AddSingleton<IConnection>(_ =>
        {
            ConnectionFactory factory = new()
            {
                HostName = config["RabbitMq:Host"] ?? "rabbitmq",
                UserName = config["RabbitMq:UserName"] ?? "guest",
                Password = config["RabbitMq:Password"] ?? "guest",
            };

            int retryCount = 0;
            while (true)
            {
                try
                {
                    return factory.CreateConnection();
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount > 5) throw;
                    Console.WriteLine($"[RabbitMQ] Čekam servis... Pokušaj {retryCount}/5");
                    Thread.Sleep(5000);
                }
            }
        });

        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddScoped<TicketCreatedConsumer>();
        services.AddHostedService<TicketCreatedListener>();
        services.AddScoped<IEmbeddingGenerator, OllamaEmbeddingGenerator>();
        services.AddSignalR();
        services.AddScoped<INotificationService, NotificationService>();

        services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));
        services.AddScoped<IEmailService, EmailService>();

        services.Configure<AdminSettings>(config.GetSection(AdminSettings.SectionName));
        services.Configure<UiSettings>(config.GetSection(UiSettings.SectionName));

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }


}
