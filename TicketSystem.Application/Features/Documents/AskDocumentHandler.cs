using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.AI;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Documents;

// 1. ISPRAVLJENO: Promenjen povratni tip interfejsa iz Result<string> u Result<DocumentAudioResponseDto>
public sealed class AskDocumentHandler(IRagService ragService, ITextToSpeechService ttsService)
    : IRequestHandler<AskDocumentCommand, Result<DocumentAudioResponseDto>>
{
    private readonly IRagService _ragService = ragService;
    private readonly ITextToSpeechService _ttsService = ttsService;

    public async Task<Result<DocumentAudioResponseDto>> Handle(AskDocumentCommand request, CancellationToken ct)
    {
        // Validacija pitanja
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Result<DocumentAudioResponseDto>.Fail(new Error("Validation.EmptyQuestion", "Pitanje ne može biti prazno."));
        }

        // Validacija strima fajla
        if (request.FileStream == null || request.FileStream.Length == 0)
        {
            return Result<DocumentAudioResponseDto>.Fail(new Error("Validation.EmptyFile", "Učitani fajl je prazan ili nepostojeći."));
        }

        // 2. Pozivamo tvoj postojeći RAG servis koji vraća Result<string> (tekst od Llame)
        Result<string> ragResult = await _ragService.ProcessDocumentAndAskAsync(
            request.FileStream,
            request.FileName,
            request.Question);

        // Ako lokalni RAG/Llama vrati grešku, odmah je prosleđujemo nazad bez pozivanja OpenAI-ja
        if (!ragResult.IsSuccess || ragResult.Data == null)
        {
            return Result<DocumentAudioResponseDto>.Fail(new Error("Validation.EmptyFile", "Učitani fajl je prazan ili nepostojeći."));
        }

        string llamaTextResponse = ragResult.Data;

        // 3. NOVI DEO: Pozivamo tvoj TextToSpeechService da pretvori tekst Llame u audio bajtove
        string audioBase64 = await _ttsService.ConvertTextToSpeechBase64Async(llamaTextResponse, request.Language);

        // 4. Pakujemo oba odgovora u zajednički DTO
        DocumentAudioResponseDto combinedData = new()
        {
            Text = llamaTextResponse,
            AudioBase64 = audioBase64
        };

        // 5. Vraćamo uspešan spakovani rezultat
        return Result<DocumentAudioResponseDto>.Success(combinedData); // Prilagodi tvom Result šablonu (npr. .Success ili .Ok)
    }
}