using System.Text.Json;

namespace TicketSystem.Application.Features.Chat;

public sealed class AiResponseDto
{
    public Guid ConversationId { get; set; }
    public string Type { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string? Content { get; set; }
    public JsonElement? Data { get; set; }
}
