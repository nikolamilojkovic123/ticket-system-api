using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.AI;
using TicketSystem.Core;
using TicketSystem.Domain.DocumentManagement.Entities;
using TicketSystem.Domain.DocumentManagement.Repositories;

namespace TicketSystem.Application.Features.Documents.Ask;

public sealed class AskStoredDocumentHandler(
    IDocumentRepository documentRepository,
    IRagService ragService,
    ITextToSpeechService ttsService)
    : IRequestHandler<AskStoredDocumentCommand, Result<DocumentAudioResponseDto>>
{
    public async Task<Result<DocumentAudioResponseDto>> Handle(AskStoredDocumentCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return Result<DocumentAudioResponseDto>.Fail(new Error("Validation.EmptyQuestion", "Pitanje ne može biti prazno."));

        Document? document = await documentRepository.GetByIdWithChunksAsync(request.DocumentId, ct);
        if (document is null)
            return Result<DocumentAudioResponseDto>.Fail(new Error("Document.NotFound", "Dokument nije pronađen."));

        IEnumerable<(string Content, float[] Embedding)> chunks = document.Chunks
            .Where(c => c.GetEmbedding() != null)
            .Select(c => (c.Content, c.GetEmbedding()!));

        Result<string> ragResult = await ragService.AskWithStoredChunksAsync(chunks, request.Question);
        if (!ragResult.IsSuccess || ragResult.Data is null)
            return Result<DocumentAudioResponseDto>.Fail(ragResult.Errors);

        string audioBase64 = await ttsService.ConvertTextToSpeechBase64Async(ragResult.Data, request.Language);

        return Result<DocumentAudioResponseDto>.Success(new DocumentAudioResponseDto
        {
            Text = ragResult.Data,
            AudioBase64 = audioBase64
        });
    }
}
