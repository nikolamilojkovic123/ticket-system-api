namespace TicketSystem.Domain.TicketManagment.Entities;

public sealed class TicketComment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = default!;
    public string Author { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private TicketComment() { }

    public TicketComment(Guid ticketId, string author, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty.", nameof(content));

        Id = Guid.NewGuid();
        TicketId = ticketId;
        Author = author;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}
