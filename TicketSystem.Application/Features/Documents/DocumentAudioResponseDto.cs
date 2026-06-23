namespace TicketSystem.Application.Features.Documents;

public sealed class DocumentAudioResponseDto
{
    public string Text { get; set; } = string.Empty;
    public string AudioBase64 { get; set; } = string.Empty;
}
