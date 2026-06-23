using TicketSystem.Domain.ConversationManagement.Entities;

namespace TicketSystem.Domain.ChatMessageManagement.Entities;

public sealed class ChatMessage
{
    public long Id { get; private set; }
    public string? Role { get; private set; } // "user", "assistant", "system"
    public string? Content { get; private set; }
    public Guid ConversationId { get; private set; }
    public DateTime? CreatedAt { get; private set; }
    public Conversation Conversation { get; set; } = default!;

    private ChatMessage() { }

    public ChatMessage(Guid conversationId, string role, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.");

        string[] validRoles = new[] { "user", "assistant", "system" };

        if (!validRoles.Contains(role.ToLower()))
            throw new ArgumentException("Invalid role.");

        ConversationId = conversationId;
        Role = role.ToLower();
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    public ChatMessage WithRole(string? role)
    {
        Role = role;
        return this;
    }
    public ChatMessage WithContent(string? content)
    {
        Content = content;
        return this;
    }
    public ChatMessage WithCreatedAt(DateTime? createdAt)
    {
        CreatedAt = createdAt;
        return this;
    }
    public ChatMessage WithConversationId(Guid conversationId)
    {
        ConversationId = conversationId;
        return this;
    }
}
