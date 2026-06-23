using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.ConversationManagement.Entities;
using TicketSystem.Domain.DocumentManagement.Entities;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.UserManagment.Entities;

namespace TicketSystem.Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
