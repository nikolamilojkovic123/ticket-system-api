using Microsoft.Extensions.Configuration;
using TicketSystem.Application.Features.Chat;
using TicketSystem.Application.Services.AI;
using System.Text;
using System.Text.Json;

namespace TicketSystem.Infrastructure.ExternalServices;

public sealed class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> SendPromptAsync(ICollection<AiMessage> messages, CancellationToken cancellationToken = default)
    {
        string? apiKey = _configuration["OpenAI:ApiKey"];

        HttpRequestMessage request = new(
            HttpMethod.Post,
            "https://api.openai.com/v1/chat/completions"
        );

        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var body = new
        {
            model = "gpt-4o-mini",
            messages = messages.Select(m => new
            {
                role = m.Role,
                content = m.Content
            })
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        using JsonDocument doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }
}
