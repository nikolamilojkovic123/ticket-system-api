using System.Text.Json;

namespace TicketSystem.Domain.DocumentManagement.Entities;

public sealed class DocumentChunk
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public int ChunkIndex { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? EmbeddingJson { get; private set; }
    public Document Document { get; private set; } = null!;

    private DocumentChunk() { }

    public DocumentChunk(Guid documentId, int chunkIndex, string content)
    {
        Id = Guid.NewGuid();
        DocumentId = documentId;
        ChunkIndex = chunkIndex;
        Content = content;
    }

    public void SetEmbedding(float[] embedding)
    {
        EmbeddingJson = JsonSerializer.Serialize(embedding);
    }

    public float[]? GetEmbedding()
    {
        if (string.IsNullOrWhiteSpace(EmbeddingJson))
            return null;

        return JsonSerializer.Deserialize<float[]>(EmbeddingJson);
    }
}
