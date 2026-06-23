using TicketSystem.Domain.TicketManagment.Enums;
using TicketSystem.Domain.UserManagment.Entities;
using System.Text.Json;

namespace TicketSystem.Domain.TicketManagment.Entities;

public sealed class Ticket
{
    private readonly List<TicketComment> _comments = new();

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TicketCategory Category { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public User Assignee { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? AiSummary { get; private set; }
    public string? Keywords { get; private set; }
    public double? SeverityScore { get; private set; }
    public string? EmbeddingJson { get; private set; }
    public IReadOnlyCollection<TicketComment> Comments => _comments.AsReadOnly();
    private Ticket() { }

    public Ticket(
        string title,
        string description,
        TicketCategory category = TicketCategory.FeatureRequest,
        TicketPriority priority = TicketPriority.Low)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Category = category;
        Priority = priority;
        Status = TicketStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid? userId)
    {
        AssigneeId = userId;
        Status = TicketStatus.InProgress;
    }

    public void ChangeStatus(TicketStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePriority(TicketPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyAiAnalysis(string summary, IEnumerable<string>? keywords, double severityScore)
    {
        if (string.IsNullOrWhiteSpace(summary))
            throw new InvalidOperationException("AI summary cannot be empty");

        AiSummary = summary;
        Keywords = keywords != null ? string.Join(",", keywords) : string.Empty;
        SeverityScore = severityScore;

        UpdatedAt = DateTime.UtcNow;
    }
    public void SetEmbedding(float[] embedding)
    {
        EmbeddingJson = JsonSerializer.Serialize(embedding);
    }

    public float[]? GetEmbedding()
    {
        if (string.IsNullOrWhiteSpace(EmbeddingJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<float[]>(EmbeddingJson);
    }
}
