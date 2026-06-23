using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TicketSystem.Domain.ConversationManagement.Repositories;
using TicketSystem.Domain.DasboardManagement;
using TicketSystem.Domain.DocumentManagement.Repositories;
using TicketSystem.Domain.TicketManagment.Repositories;
using TicketSystem.Domain.UserManagment.Repositories;
using TicketSystem.Infrastructure.Database.Repositories;

namespace TicketSystem.Infrastructure.Database;

public static class DependencyInjection
{
    const string _connectionName = "DefaultConnection";

    public static void AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            string? connectionString = configuration.GetConnectionString(_connectionName);
            options.UseSqlServer(connectionString);
        });
        builder.Services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.TryAddScoped<ITicketRepository, TicketRepository>();
        services.TryAddScoped<IConversationRepository, ConversationRepository>();
        services.TryAddScoped<IDashboardRepository, DashboardRepository>();
        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IDocumentRepository, DocumentRepository>();
    }


}
