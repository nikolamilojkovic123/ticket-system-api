using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Documents.Upload;

public sealed record UploadDocumentCommand(Stream FileStream, string FileName) : IRequest<Result<Guid>>;
