using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Models;
using TicketSystem.Domain.TicketManagment.Repositories;

namespace TicketSystem.Infrastructure.Database.Repositories;

internal sealed class TicketRepository(ApplicationDbContext dbContext) : ITicketRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        await _dbContext.Tickets.AddAsync(ticket, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<Ticket>> GetAllTicketsAsync(CancellationToken cancellationToken)
    =>
       await _dbContext.Tickets.ToListAsync(cancellationToken);

    public async Task<(List<Ticket> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Ticket> query = _dbContext.Tickets
            .Include(x => x.Assignee)
            .AsNoTracking();
        int totalCount = await query.CountAsync(ct);

        List<Ticket> items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Ticket?> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken)
    =>
       await _dbContext.Tickets
            .Include(x => x.Assignee)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public IQueryable<Ticket> Query()
    =>
        _dbContext.Tickets.AsNoTracking();

    public async Task<(ICollection<Ticket> Items, int TotalCount)> SearchSimilarTicketsAsync(
        float[] queryVector,
        float threshold,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        List<(Ticket Ticket, float Similarity)> matchedTickets = [];

        List<Ticket> tickets = await _dbContext.Tickets
            .AsNoTracking()
            .Where(x => x.EmbeddingJson != null)
            .ToListAsync(cancellationToken);

        foreach (Ticket ticket in tickets)
        {
            float[]? embedding = ticket.GetEmbedding();

            if (embedding == null)
            {
                continue;
            }

            float similarity = CalculateCosineSimilarity(
                queryVector,
                embedding);

            if (similarity < threshold)
            {
                continue;
            }

            matchedTickets.Add((ticket, similarity));
        }

        List<Ticket> pagedItems = matchedTickets
            .OrderByDescending(x => x.Similarity)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Ticket)
            .ToList();

        return (
            Items: pagedItems,
            TotalCount: matchedTickets.Count
        );
    }

    public async Task<(List<Ticket> Items, int TotalCount)> GetFilteredAsync(TicketFilterParams filter, CancellationToken ct)
    {
        IQueryable<Ticket> query = _dbContext.Tickets
            .Include(x => x.Assignee)
            .AsNoTracking();

        if (filter.Statuses.Count > 0)
            query = query.Where(x => filter.Statuses.Contains(x.Status));

        if (filter.Priorities.Count > 0)
            query = query.Where(x => filter.Priorities.Contains(x.Priority));

        if (filter.Categories.Count > 0)
            query = query.Where(x => filter.Categories.Contains(x.Category));

        if (filter.AssigneeId.HasValue)
            query = query.Where(x => x.AssigneeId == filter.AssigneeId);

        if (filter.DateFrom.HasValue)
            query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(x => x.CreatedAt <= filter.DateTo.Value.AddDays(1));

        int totalCount = await query.CountAsync(ct);

        List<Ticket> items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken ct)
    {
        _dbContext.Tickets.Update(ticket);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<(ICollection<Ticket> Items, int TotalCount)> SearchByKeywordAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Ticket> baseQuery = _dbContext.Tickets
       .AsNoTracking()
       .Where(x =>
           x.Title.Contains(query) ||
           x.Description.Contains(query));

        int totalCount = await baseQuery.CountAsync(ct);

        List<Ticket> items = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddCommentAsync(TicketComment comment, CancellationToken ct)
    {
        await _dbContext.TicketComments.AddAsync(comment, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<TicketComment>> GetCommentsAsync(Guid ticketId, CancellationToken ct)
    =>
        await _dbContext.TicketComments
            .AsNoTracking()
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);

    private static float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vektori moraju biti iste dužine.");
        }

        float dotProduct = 0f;
        float normA = 0f;
        float normB = 0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }

        if (normA == 0f || normB == 0f)
            return 0f;

        return dotProduct / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
    }


}
