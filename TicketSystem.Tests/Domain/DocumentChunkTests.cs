using FluentAssertions;
using TicketSystem.Domain.DocumentManagement.Entities;

namespace TicketSystem.Tests.Domain;

public class DocumentChunkTests
{
    [Fact]
    public void SetEmbedding_StoresAndRetrievesCorrectly()
    {
        var chunk = new DocumentChunk(Guid.NewGuid(), 0, "Sadržaj chunka");
        var embedding = new float[] { 0.1f, 0.5f, -0.3f, 0.9f };

        chunk.SetEmbedding(embedding);
        var retrieved = chunk.GetEmbedding();

        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(embedding);
    }

    [Fact]
    public void GetEmbedding_NoEmbeddingSet_ReturnsNull()
    {
        var chunk = new DocumentChunk(Guid.NewGuid(), 0, "Sadržaj");

        chunk.GetEmbedding().Should().BeNull();
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var documentId = Guid.NewGuid();
        var chunk = new DocumentChunk(documentId, 3, "Tekst chunka");

        chunk.Id.Should().NotBeEmpty();
        chunk.DocumentId.Should().Be(documentId);
        chunk.ChunkIndex.Should().Be(3);
        chunk.Content.Should().Be("Tekst chunka");
    }
}

public class DocumentTests
{
    [Fact]
    public void Constructor_ValidFileName_CreatesDocument()
    {
        var document = new Document("test.pdf");

        document.Id.Should().NotBeEmpty();
        document.FileName.Should().Be("test.pdf");
        document.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        document.Chunks.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyFileName_ThrowsException(string fileName)
    {
        var act = () => new Document(fileName);

        act.Should().Throw<ArgumentException>().WithMessage("*FileName*");
    }
}
