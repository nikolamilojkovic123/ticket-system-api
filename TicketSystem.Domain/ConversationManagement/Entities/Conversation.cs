using TicketSystem.Domain.ChatMessageManagement.Entities;

namespace TicketSystem.Domain.ConversationManagement.Entities;

public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = new();
    public Guid Id { get; set; }
    public string Title { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    private Conversation() { }

    public Conversation(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title must be provided.");

        Id = Guid.NewGuid();
        Title = title;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddMessage(string role, string content)
    {
        ChatMessage message = new(this.Id, role, content);
        _messages.Add(message);
    }

    public void Rename(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("New title cannot be empty.");

        Title = newTitle;
    }
}
