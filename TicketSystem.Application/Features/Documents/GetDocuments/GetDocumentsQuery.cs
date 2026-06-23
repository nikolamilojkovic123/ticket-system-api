using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Documents.GetDocuments;

public sealed record GetDocumentsQuery : IRequest<Result<ICollection<DocumentListItemDto>>>;
