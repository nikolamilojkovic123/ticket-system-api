namespace TicketSystem.Application.Common.Collections;

public abstract class EntityCollectionResult<T>
{
    protected EntityCollectionResult(IEnumerable<T> items)
    {
        Items = [.. items];
        TotalCount = Items.Count;
    }
    public IReadOnlyCollection<T> Items { get; private set; }
    public int TotalCount { get; private set; }
}
