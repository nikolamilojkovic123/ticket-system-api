using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Models;

namespace TicketSystem.Domain.TicketManagment.Repositories;

public interface ITicketRepository
{
    Task CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken);
    Task UpdateAsync(Ticket ticket, CancellationToken ct);
    Task<Ticket?> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ICollection<Ticket>> GetAllTicketsAsync(CancellationToken cancellationToken);
    IQueryable<Ticket> Query();
    Task<(List<Ticket> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<(List<Ticket> Items, int TotalCount)> GetFilteredAsync(TicketFilterParams filter, CancellationToken ct);
    Task<(ICollection<Ticket> Items, int TotalCount)> SearchSimilarTicketsAsync(float[] queryVector, float threshold, int page, int pageSize, CancellationToken cancellationToken);
    Task<(ICollection<Ticket> Items, int TotalCount)> SearchByKeywordAsync(
    string query,
    int page,
    int pageSize,
    CancellationToken ct);
    Task AddCommentAsync(TicketComment comment, CancellationToken ct);
    Task<List<TicketComment>> GetCommentsAsync(Guid ticketId, CancellationToken ct);
}
