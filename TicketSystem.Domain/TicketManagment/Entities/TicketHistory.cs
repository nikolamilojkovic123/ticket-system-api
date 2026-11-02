namespace TicketSystem.Domain.TicketManagment.Entities;

/// <summary>One recorded change of a single ticket field.</summary>
public sealed class TicketHistory
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Field { get; private set; } = default!;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string ChangedBy { get; private set; } = default!;
    public DateTime ChangedAt { get; private set; }

    private TicketHistory() { }

    internal TicketHistory(Guid ticketId, string field, string? oldValue, string? newValue, string changedBy)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field is required.", nameof(field));

        Id = Guid.NewGuid();
        TicketId = ticketId;
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedBy = string.IsNullOrWhiteSpace(changedBy) ? TicketHistoryFields.SystemUser : changedBy;
        ChangedAt = DateTime.UtcNow;
    }

    internal void ReplaceNewValue(string? newValue) => NewValue = newValue;
}
