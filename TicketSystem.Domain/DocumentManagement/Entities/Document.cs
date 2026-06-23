namespace TicketSystem.Domain.DocumentManagement.Entities;

public sealed class Document
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }
    public ICollection<DocumentChunk> Chunks { get; private set; } = new List<DocumentChunk>();

    private Document() { }

    public Document(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName is required.", nameof(fileName));

        Id = Guid.NewGuid();
        FileName = fileName;
        UploadedAt = DateTime.UtcNow;
    }
}
