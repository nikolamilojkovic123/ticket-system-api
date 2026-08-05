using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Documents.Ask;

public sealed record AskStoredDocumentCommand(Guid DocumentId, string Question, string Voice)
    : IRequest<Result<DocumentAudioResponseDto>>;
