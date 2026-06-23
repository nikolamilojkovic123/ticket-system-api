using TicketSystem.Application.Features.Chat;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;
using System.Text.Json;

namespace TicketSystem.Application.Services.AI;

public sealed class TicketAiService(
    IOpenAiService openAiService,
    ITicketRepository ticketRepository,
    IEmbeddingGenerator embeddingGenerator)
    : ITicketAiService
{
    private readonly IOpenAiService _openAi = openAiService
        ?? throw new ArgumentNullException(nameof(openAiService));

    private readonly ITicketRepository _ticketRepository = ticketRepository
        ?? throw new ArgumentNullException(nameof(ticketRepository));
    private readonly IEmbeddingGenerator _embeddingGenerator = embeddingGenerator
        ?? throw new ArgumentNullException(nameof(embeddingGenerator));
    public async Task<bool?> EnrichTicketAsync(Guid ticketId, CancellationToken ct = default)
    {
        Ticket? ticket = await _ticketRepository.GetTicketByIdAsync(ticketId, ct);

        if (ticket == null)
        {
            return null;
        }

        ICollection<AiMessage> messages = BuildMessages(ticket);

        string rawResponse = await _openAi.SendPromptAsync(messages, ct);

        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return null;
        }

        string cleanJson = CleanJson(rawResponse);

        TicketAiAnalysisDto? analysis;

        try
        {
            analysis = JsonSerializer.Deserialize<TicketAiAnalysisDto>(
                cleanJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            return null;
        }

        if (analysis == null)
        {
            return null;
        }

        ticket.ApplyAiAnalysis(
            analysis.AiSummary,
            analysis.Keywords,
            analysis.SeverityScore);
        string textToEmbed = $"{ticket.Title} - {ticket.Description}";
        float[] embeddingArray = await _embeddingGenerator.GenerateEmbeddingAsync(textToEmbed);

        ticket.SetEmbedding(embeddingArray);

        await _ticketRepository.UpdateAsync(ticket, ct);

        return true;
    }

    private static ICollection<AiMessage> BuildMessages(Ticket ticket)
    {
        return
        [
            new AiMessage
            {
                Role = "system",
                Content =
                """
                You are a strict ticket classification system.

                RULES:
                - Return ONLY valid JSON
                - Do not use markdown
                - Do not add explanations
                - Do not add extra fields
                - Return EXACTLY one JSON object

                JSON SCHEMA:
                {
                  "category": "Bug | FeatureRequest | TechnicalIssue | Billing | Other",
                  "priority": "Low | Medium | High",
                  "aiSummary": "string",
                  "keywords": ["string"],
                  "severityScore": 0.0
                }

                VALIDATION:
                - severityScore must be between 0 and 1
                - keywords must contain 3-7 short strings
                - aiSummary should be 1-2 short sentences
                """
            },

            new AiMessage
            {
                Role = "user",
                Content =
                $"""
                Analyze this ticket.

                Title:
                {ticket.Title}

                Description:
                {ticket.Description}
                """
            }
        ];
    }

    private static string CleanJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "{}";
        }

        return input
            .Replace("```json", string.Empty)
            .Replace("```", string.Empty)
            .Trim();
    }
}

