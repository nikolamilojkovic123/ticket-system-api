using TicketSystem.Domain.TicketManagment.Enums;
using TicketSystem.Domain.UserManagment.Entities;
using System.Text.Json;

namespace TicketSystem.Domain.TicketManagment.Entities;

public sealed class Ticket
{
    private readonly List<TicketComment> _comments = new();
    private readonly List<TicketHistory> _pendingHistory = new();

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

    /// <summary>History entries recorded since the ticket was loaded and not yet persisted.</summary>
    public IReadOnlyList<TicketHistory> PendingHistory => _pendingHistory.AsReadOnly();
    private Ticket() { }

    public Ticket(
        string title,
        string description,
        TicketCategory category = TicketCategory.FeatureRequest,
        TicketPriority priority = TicketPriority.Low,
        string? createdBy = null)
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

        _pendingHistory.Add(new TicketHistory(Id, TicketHistoryFields.Created, null, title, createdBy ?? TicketHistoryFields.SystemUser));
    }

    /// <summary>
    /// Assigns the ticket. Assigning to a (new) user also moves the ticket to InProgress.
    /// </summary>
    /// <param name="assigneeName">Display name of the new assignee, stored in the history.</param>
    public void AssignTo(Guid? userId, string? assigneeName = null, string? changedBy = null)
    {
        if (userId == AssigneeId)
            return;

        string? previousName = Assignee?.FullName;

        AssigneeId = userId;
        UpdatedAt = DateTime.UtcNow;
        Record(TicketHistoryFields.Assignee, previousName, userId.HasValue ? assigneeName : null, changedBy);

        if (userId.HasValue)
            ChangeStatus(TicketStatus.InProgress, changedBy);
    }

    public void ChangeStatus(TicketStatus newStatus, string? changedBy = null)
    {
        Record(TicketHistoryFields.Status, Status.ToString(), newStatus.ToString(), changedBy);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePriority(TicketPriority priority, string? changedBy = null)
    {
        Record(TicketHistoryFields.Priority, Priority.ToString(), priority.ToString(), changedBy);
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description, string? changedBy = null)
    {
        Record(TicketHistoryFields.Title, Title, title, changedBy);
        Record(TicketHistoryFields.Description, Description, description, changedBy);
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Clears the pending history once the repository has persisted it.</summary>
    public void ClearPendingHistory() => _pendingHistory.Clear();

    /// <summary>
    /// Adds a history entry for a real change. Several changes of the same field before saving
    /// collapse into one entry (first old value -> last new value), and drop out if they cancel out.
    /// </summary>
    private void Record(string field, string? oldValue, string? newValue, string? changedBy)
    {
        TicketHistory? pending = _pendingHistory.FirstOrDefault(h => h.Field == field);

        if (pending is not null)
        {
            pending.ReplaceNewValue(newValue);

            if (pending.OldValue == pending.NewValue)
                _pendingHistory.Remove(pending);

            return;
        }

        if (oldValue == newValue)
            return;

        _pendingHistory.Add(new TicketHistory(Id, field, oldValue, newValue, changedBy ?? TicketHistoryFields.SystemUser));
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
