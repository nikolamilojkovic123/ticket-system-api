using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.AI;
using TicketSystem.Domain.ConversationManagement.Entities;
using TicketSystem.Domain.ConversationManagement.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketSystem.Application.Features.Chat;

public sealed class GenerateAiResponseHandler(
    IOpenAiService openAiService,
    IConversationRepository conversationRepository)
    : IRequestHandler<GenerateAiResponseCommand, AiResponseDto>
{
    private readonly IOpenAiService _openAi = openAiService;
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task<AiResponseDto> Handle(GenerateAiResponseCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        Conversation conversation = await GetOrCreateConversation(request.ConversationId, ct);

        conversation.AddMessage("user", request.Prompt);

        if (conversation.Messages.Count == 1)
        {
            await SetConversationTitle(conversation, request.Prompt, ct);
        }

        List<AiMessage> aiMessages = BuildAiMessages(conversation);

        string rawResponse = await _openAi.SendPromptAsync(aiMessages, ct);
        string cleanJson = CleanJson(rawResponse);

        AiResponseDto response = DeserializeResponse(cleanJson);

        string storedMessage = response.Type == "message"
            ? response.Content ?? cleanJson
            : cleanJson;

        conversation.AddMessage("assistant", storedMessage);

        await _conversationRepository.UpdateAsync(conversation, ct);

        response.ConversationId = conversation.Id;
        return response;
    }

    private async Task<Conversation> GetOrCreateConversation(Guid? conversationId, CancellationToken ct)
    {
        if (conversationId is null || conversationId == Guid.Empty)
        {
            var newConv = new Conversation("New Conversation...");
            await _conversationRepository.AddAsync(newConv, ct);
            return newConv;
        }

        Conversation? existing = await _conversationRepository.GetConversationByIdAsync(conversationId.Value, ct);

        return existing ?? throw new InvalidOperationException("Conversation not found");
    }

    private static List<AiMessage> BuildAiMessages(Conversation conversation)
    {
        var messages = conversation.Messages.Select(m => new AiMessage
        {
            Role = m.Role,
            Content = m.Content
        }).ToList();

        messages.Insert(0, new AiMessage
        {
            Role = "system",
            Content =
"""
You are a strict JSON generator for an AI assistant.

RULES (ABSOLUTE):
- You MUST return EXACTLY ONE JSON object
- NEVER return multiple objects
- NEVER mix message and action
- NEVER output explanations or text outside JSON
- Output must be valid JSON only

DECISION RULE:
- If user is chatting, answering, explaining → type = "message"
- If user requests creation or system action → type = "action"

SCHEMA 1 - MESSAGE:
{
  "type": "message",
  "content": "string"
}

SCHEMA 2 - ACTION:
{
  "type": "action",
  "action": "create_ticket",
  "data": {
    "title": "string or null",
    "description": "string or null",
    "category": "LoginIssue | Billing | TechnicalIssue | BugReport | FeatureRequest | null",
    "priority": "High | Medium | Low | null"
  }
}

CRITICAL:
- Return ONLY ONE schema per response
- Do not combine schemas
- Do not include both message and action
"""
        });

        return messages;
    }

    private static AiResponseDto DeserializeResponse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<AiResponseDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                })!;
        }
        catch
        {
            return new AiResponseDto
            {
                Type = "message",
                Content = json
            };
        }
    }

    private static string CleanJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "{}";

        return input
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
    }

    private async Task SetConversationTitle(Conversation conv, string prompt, CancellationToken ct)
    {
        var titlePrompt = new List<AiMessage>
        {
            new() { Role = "system", Content = "Create max 4 word title. Return only text." },
            new() { Role = "user", Content = prompt }
        };

        string title = await _openAi.SendPromptAsync(titlePrompt, ct);

        conv.Rename(title.Trim('"', ' ', '.'));
    }
}