using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Application.Services.AI;
using TicketSystem.Core;
using TicketSystem.Domain.DocumentManagement.Entities;
using TicketSystem.Domain.DocumentManagement.Repositories;

namespace TicketSystem.Application.Features.Documents.Upload;

public sealed class UploadDocumentHandler(
    IDocumentRepository documentRepository,
    IRagService ragService,
    IEmbeddingGenerator embeddingGenerator)
    : IRequestHandler<UploadDocumentCommand, Result<UploadDocumentResponse>>
{
    public async Task<Result<UploadDocumentResponse>> Handle(UploadDocumentCommand request, CancellationToken ct)
    {
        if (request.FileStream == null || request.FileStream.Length == 0)
            return Result<UploadDocumentResponse>.Fail(new ErrorData("Upload.EmptyFile", "File is empty"));

        string fullText = await ragService.ExtractTextAsync(request.FileStream, request.FileName);
        if (string.IsNullOrWhiteSpace(fullText))
            return Result<UploadDocumentResponse>.Fail(new ErrorData("Upload.EmptyContent", "Document doesn't has a text"));

        List<string> chunks = ragService.ChunkText(fullText);

        Document document = new(request.FileName);

        for (int i = 0; i < chunks.Count; i++)
        {
            DocumentChunk chunk = new(document.Id, i, chunks[i]);
            float[] embedding = await embeddingGenerator.GenerateEmbeddingAsync(chunks[i]);
            chunk.SetEmbedding(embedding);
            document.Chunks.Add(chunk);
        }

        await documentRepository.SaveAsync(document, ct);

        return Result<UploadDocumentResponse>.Success(new()
        {
            DocumentId = document.Id
        });
    }
}
