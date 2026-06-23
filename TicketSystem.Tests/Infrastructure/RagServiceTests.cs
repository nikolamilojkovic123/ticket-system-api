using FluentAssertions;
using TicketSystem.Infrastructure.ExternalServices;

namespace TicketSystem.Tests.Infrastructure;

public class RagServiceTests
{
    private readonly OllamaRagService _sut = new();

    [Fact]
    public void ChunkText_ShortText_ReturnsSingleChunk()
    {
        var text = "Ovo je kratak tekst.";

        var chunks = _sut.ChunkText(text, maxCharacters: 1000);

        chunks.Should().HaveCount(1);
        chunks[0].Should().Contain("Ovo je kratak tekst.");
    }

    [Fact]
    public void ChunkText_LongText_SplitsIntoMultipleChunks()
    {
        var paragraph = new string('a', 600);
        var text = $"{paragraph}\n{paragraph}\n{paragraph}";

        var chunks = _sut.ChunkText(text, maxCharacters: 1000);

        chunks.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ChunkText_EmptyText_ReturnsEmptyList()
    {
        var chunks = _sut.ChunkText("", maxCharacters: 1000);

        chunks.Should().BeEmpty();
    }

    [Fact]
    public void ChunkText_PreservesAllContent()
    {
        var lines = Enumerable.Range(1, 20).Select(i => $"Linija {i} teksta.").ToList();
        var text = string.Join("\n", lines);

        var chunks = _sut.ChunkText(text, maxCharacters: 100);
        var combined = string.Join("", chunks);

        foreach (var line in lines)
            combined.Should().Contain(line);
    }

    [Fact]
    public void ChunkText_NoChunkExceedsMaxCharacters()
    {
        var paragraph = new string('x', 300);
        var text = string.Join("\n", Enumerable.Repeat(paragraph, 10));

        var chunks = _sut.ChunkText(text, maxCharacters: 500);

        foreach (var chunk in chunks)
            chunk.Length.Should().BeLessThanOrEqualTo(600);
    }

    [Theory]
    [InlineData(new float[] { 1, 0 }, new float[] { 1, 0 }, 1.0)]
    [InlineData(new float[] { 1, 0 }, new float[] { 0, 1 }, 0.0)]
    [InlineData(new float[] { 1, 1 }, new float[] { 1, 1 }, 1.0)]
    public void CosineSimilarity_KnownVectors_ReturnsExpectedResult(
        float[] v1, float[] v2, double expected)
    {
        var method = typeof(OllamaRagService)
            .GetMethod("CosineSimilarity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (double)method!.Invoke(_sut, new object[] { v1, v2 })!;

        result.Should().BeApproximately(expected, precision: 0.0001);
    }

    [Fact]
    public void CosineSimilarity_IdenticalVectors_ReturnsOne()
    {
        var vector = new float[] { 0.5f, 0.3f, 0.8f, 0.1f };

        var method = typeof(OllamaRagService)
            .GetMethod("CosineSimilarity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (double)method!.Invoke(_sut, new object[] { vector, vector })!;

        result.Should().BeApproximately(1.0, precision: 0.0001);
    }
}
