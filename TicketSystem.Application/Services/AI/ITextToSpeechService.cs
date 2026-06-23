namespace TicketSystem.Application.Services.AI;

public interface ITextToSpeechService
{
    Task<string> ConvertTextToSpeechBase64Async(string text, string language);
}
