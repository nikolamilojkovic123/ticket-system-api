using Microsoft.Extensions.AI;

namespace TicketSystem.Infrastructure.ExternalServices
{
    public sealed class OllamaEmbeddingGenerator : Application.Services.IEmbeddingGenerator
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

        public OllamaEmbeddingGenerator()
        {
            _generator = new Microsoft.Extensions.AI.OllamaEmbeddingGenerator(
                new Uri("http://127.0.0.1:11434"),
                modelId: "nomic-embed-text");
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            (string Value, Embedding<float> Embedding)[] result = await _generator.GenerateAndZipAsync(new List<string> { text });

            return result[0].Embedding.Vector.ToArray();
        }
    }
}
