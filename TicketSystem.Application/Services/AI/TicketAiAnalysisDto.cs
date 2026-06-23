namespace TicketSystem.Application.Services.AI;

public sealed class TicketAiAnalysisDto
{
    public string Category { get; set; } = default!;

    public string Priority { get; set; } = default!;

    public string AiSummary { get; set; } = default!;

    public ICollection<string> Keywords { get; set; } = [];

    public double SeverityScore { get; set; }
}
