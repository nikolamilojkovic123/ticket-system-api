namespace TicketSystem.Core;

public sealed class ErrorData(string code, string message)
{
    public string Code { get; private set; } = code ?? throw new ArgumentNullException(nameof(code));
    public string Message { get; private set; } = message ?? throw new ArgumentNullException(nameof(message));

    public static readonly ErrorData None = new("None", "None");

    public override bool Equals(object? obj)
    {
        if (obj is not ErrorData other) return false;
        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode() => HashCode.Combine(Code, Message);
}
