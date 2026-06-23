namespace TicketSystem.Application.Features.Documents.GetDocuments;

public sealed class DocumentListItemDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
    public int ChunkCount { get; init; }
}
