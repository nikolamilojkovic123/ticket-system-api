using TicketSystem.Core;

namespace TicketSystem.Application.Services.AI;

public interface IRagService
{
    Task<Result<string>> ProcessDocumentAndAskAsync(Stream fileStream, string fileName, string question);
    Task<Result<string>> AskWithStoredChunksAsync(IEnumerable<(string Content, float[] Embedding)> chunks, string question);
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
    List<string> ChunkText(string text, int maxCharacters = 1000);
}
