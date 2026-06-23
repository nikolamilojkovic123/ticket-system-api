using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.DocumentManagement.Entities;
using TicketSystem.Domain.DocumentManagement.Repositories;

namespace TicketSystem.Infrastructure.Database.Repositories;

public sealed class DocumentRepository(ApplicationDbContext dbContext) : IDocumentRepository
{
    public async Task<Guid> SaveAsync(Document document, CancellationToken ct)
    {
        await dbContext.Documents.AddAsync(document, ct);
        await dbContext.SaveChangesAsync(ct);
        return document.Id;
    }

    public async Task<Document?> GetByIdWithChunksAsync(Guid documentId, CancellationToken ct)
        => await dbContext.Documents
            .Include(d => d.Chunks)
            .FirstOrDefaultAsync(d => d.Id == documentId, ct);

    public async Task<List<Document>> GetAllAsync(CancellationToken ct)
        => await dbContext.Documents
            .Include(d => d.Chunks)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(ct);
}
