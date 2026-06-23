using TicketSystem.Domain.DocumentManagement.Entities;

namespace TicketSystem.Domain.DocumentManagement.Repositories;

public interface IDocumentRepository
{
    Task<Guid> SaveAsync(Document document, CancellationToken ct);
    Task<Document?> GetByIdWithChunksAsync(Guid documentId, CancellationToken ct);
    Task<List<Document>> GetAllAsync(CancellationToken ct);
}
