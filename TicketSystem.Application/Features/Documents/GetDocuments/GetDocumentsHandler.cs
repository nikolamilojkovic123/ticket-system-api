using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.DocumentManagement.Entities;
using TicketSystem.Domain.DocumentManagement.Repositories;

namespace TicketSystem.Application.Features.Documents.GetDocuments;

public sealed class GetDocumentsHandler(IDocumentRepository documentRepository)
    : IRequestHandler<GetDocumentsQuery, Result<ICollection<DocumentListItemDto>>>
{
    public async Task<Result<ICollection<DocumentListItemDto>>> Handle(GetDocumentsQuery request, CancellationToken ct)
    {
        List<Document> documents = await documentRepository.GetAllAsync(ct);

        List<DocumentListItemDto> result = documents.Select(d => new DocumentListItemDto
        {
            Id = d.Id,
            FileName = d.FileName,
            UploadedAt = d.UploadedAt,
            ChunkCount = d.Chunks.Count
        }).ToList();

        return Result<ICollection<DocumentListItemDto>>.Success(result);
    }
}
