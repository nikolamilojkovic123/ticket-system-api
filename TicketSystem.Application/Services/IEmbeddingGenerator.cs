namespace TicketSystem.Application.Services;

public interface IEmbeddingGenerator
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
