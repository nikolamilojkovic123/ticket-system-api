using Microsoft.Extensions.AI;
using TicketSystem.Application.Services.AI;
using TicketSystem.Core;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace TicketSystem.Infrastructure.ExternalServices;

public sealed class OllamaRagService : IRagService
{
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public OllamaRagService(HttpClient httpClient) : this() { }

    public OllamaRagService()
    {
        _chatClient = new OllamaChatClient(new Uri("http://ollama:11434"), modelId: "llama3");
        _embeddingGenerator = new Microsoft.Extensions.AI.OllamaEmbeddingGenerator(
            new Uri("http://ollama:11434"),
            modelId: "nomic-embed-text");
    }

    public async Task<Result<string>> ProcessDocumentAndAskAsync(Stream fileStream, string fileName, string question)
    {
        try
        {
            string fullText = await ExtractTextAsync(fileStream, fileName);
            if (string.IsNullOrWhiteSpace(fullText))
                return Result<string>.Fail(new Error("RAG.EmptyDocument", "Dokument je prazan ili nečitljiv."));

            List<string> chunks = ChunkText(fullText);

            (string Value, Embedding<float> Embedding)[] questionEmbeddingResult =
                await _embeddingGenerator.GenerateAndZipAsync(new List<string> { question });
            float[] questionVector = questionEmbeddingResult[0].Embedding.Vector.ToArray();

            List<string> relevantChunks = await FindRelevantChunksAsync(chunks, questionVector, topK: 3);

            return await AskLlmAsync(relevantChunks, question);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail(new Error("RAG.ExecutionError", ex.Message));
        }
    }

    public async Task<Result<string>> AskWithStoredChunksAsync(
        IEnumerable<(string Content, float[] Embedding)> chunks, string question)
    {
        try
        {
            (string Value, Embedding<float> Embedding)[] questionResult =
                await _embeddingGenerator.GenerateAndZipAsync(new List<string> { question });
            float[] questionVector = questionResult[0].Embedding.Vector.ToArray();

            List<string> relevantChunks = chunks
                .Select(c => (c.Content, Score: CosineSimilarity(questionVector, c.Embedding)))
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => x.Content)
                .ToList();

            return await AskLlmAsync(relevantChunks, question);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail(new Error("RAG.ExecutionError", ex.Message));
        }
    }

    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            using StreamReader reader = new(fileStream);
            return await reader.ReadToEndAsync();
        }

        using PdfDocument pdf = PdfDocument.Open(fileStream);
        StringBuilder textBuilder = new();
        foreach (Page? page in pdf.GetPages())
            textBuilder.AppendLine(page.Text);

        return textBuilder.ToString();
    }

    public List<string> ChunkText(string text, int maxCharacters = 1000)
    {
        string[] paragraphs = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        List<string> chunks = new();
        StringBuilder currentChunk = new();

        foreach (string p in paragraphs)
        {
            if (currentChunk.Length + p.Length > maxCharacters)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }
            currentChunk.AppendLine(p);
        }

        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());

        return chunks;
    }

    private async Task<List<string>> FindRelevantChunksAsync(List<string> chunks, float[] questionVector, int topK)
    {
        (string Value, Embedding<float> Embedding)[] zippedEmbeddings =
            await _embeddingGenerator.GenerateAndZipAsync(chunks);

        return zippedEmbeddings
            .Select(item => (item.Value, Score: CosineSimilarity(questionVector, item.Embedding.Vector.ToArray())))
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Value)
            .ToList();
    }

    private async Task<Result<string>> AskLlmAsync(List<string> relevantChunks, string question)
    {
        string context = string.Join("\n\n", relevantChunks);
        string prompt = $@"Koristi isključivo sledeće isečke iz dokumenta da odgovoriš na pitanje korisnika na kraju.
Ako u tekstu nema odgovora, reci iskreno da ne znaš. Budi kratak i precizan.

--- POČETAK DOKUMENTA ---
{context}
--- KRAJ DOKUMENTA ---

PITANJE KORISNIKA: {question}
ODGOVOR:";

        ChatResponse response = await _chatClient.GetResponseAsync(prompt);

        string aiAnswer = response.Messages?.Count > 0
            ? response.Messages[0].Text ?? "Nisam uspeo da generišem odgovor."
            : "Model nije vratio nijedan odgovor.";

        return Result<string>.Success(aiAnswer);
    }

    private double CosineSimilarity(float[] v1, float[] v2)
    {
        double dotProduct = 0.0, normA = 0.0, normB = 0.0;
        for (int i = 0; i < v1.Length; i++)
        {
            dotProduct += v1[i] * v2[i];
            normA += Math.Pow(v1[i], 2);
            normB += Math.Pow(v2[i], 2);
        }
        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
