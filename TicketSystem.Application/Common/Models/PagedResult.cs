namespace TicketSystem.Application.Common.Models;

public sealed class PagedResult<T>
{
    public ICollection<T> Items { get; set; } = [];
    public int TotalCount { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
}
