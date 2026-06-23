using TicketSystem.Application.Features.Chat;

namespace TicketSystem.Application.Services.AI;

public interface IOpenAiService
{
    Task<string> SendPromptAsync(ICollection<AiMessage> messages, CancellationToken cancellationToken = default);

}
