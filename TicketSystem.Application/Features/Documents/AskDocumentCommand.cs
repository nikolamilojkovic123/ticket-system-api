using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Documents;

public sealed record AskDocumentCommand(
    Stream FileStream,
    string FileName,
    string Question,
    string Voice) : IRequest<Result<DocumentAudioResponseDto>>;
