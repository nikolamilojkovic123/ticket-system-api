namespace TicketSystem.Application.Features.Dashboard.Queries;

public sealed class KeywordDto
{
    public string Keyword { get; set; } = default!;
    public int Count { get; set; }
}
