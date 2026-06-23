var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TicketSystem_API>("ticketsystem-api");

builder.Build().Run();
