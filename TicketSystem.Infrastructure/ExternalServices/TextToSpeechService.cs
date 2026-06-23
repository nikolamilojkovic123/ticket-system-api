using Microsoft.Extensions.Configuration;
using TicketSystem.Application.Services.AI;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TicketSystem.Infrastructure.ExternalServices;

public class TextToSpeechService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    : ITextToSpeechService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> ConvertTextToSpeechBase64Async(string text, string language)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        string? apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return string.Empty;
        }

        using HttpClient client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        string chosenVoice = language.ToLower() switch
        {
            "sr" => "onyx",
            "en" => "alloy",
            _ => "onyx"
        };

        var payload = new
        {
            model = "tts-1",
            input = text,
            voice = chosenVoice,
            response_format = "mp3"
        };

        string jsonPayload = JsonSerializer.Serialize(payload);
        using StringContent content = new(jsonPayload, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
        return Convert.ToBase64String(audioBytes);
    }
}