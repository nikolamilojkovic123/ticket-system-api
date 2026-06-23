using TicketSystem.Application.Mediator;

namespace TicketSystem.Application.Features.Chat;

public class GenerateAiResponseCommand : IRequest<AiResponseDto>
{
    public Guid? ConversationId { get; set; }
    public string? Prompt { get; set; }
}
