using Microsoft.Extensions.Caching.Hybrid;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Features.Tickets.Queries.SearchTickets;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Repositories;

public sealed class SearchTicketsQueryHandler(
    IEmbeddingGenerator embeddingGenerator,
    ITicketRepository ticketRepository,
    HybridCache cache)
    : IRequestHandler<SearchTicketsQuery, Result<PagedResult<TicketResponse>>>
{
    private readonly IEmbeddingGenerator _embeddingGenerator =
        embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
    private readonly ITicketRepository _ticketRepository =
        ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

    public Task<Result<PagedResult<TicketResponse>>> Handle(
        SearchTicketsQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<PagedResult<TicketResponse>>> HandleInnerAsync(
        SearchTicketsQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Empty(request);
        }

        string key = $"embedding:{request.Query.Trim().ToLowerInvariant()}";


        float[] queryVector = await cache.GetOrCreateAsync(
          key,
          async token =>
          {
              return await _embeddingGenerator.GenerateEmbeddingAsync(request.Query);
          },
          cancellationToken: ct
      );
        const int candidatePool = 100;

        (ICollection<Ticket> Items, int TotalCount) semanticResult =
            await _ticketRepository.SearchSimilarTicketsAsync(
                queryVector,
                threshold: 0.75f,
                page: 1,
                pageSize: candidatePool,
                cancellationToken: ct);

        (ICollection<Ticket> Items, int TotalCount) keywordResult =
            await _ticketRepository.SearchByKeywordAsync(
                request.Query,
                page: 1,
                pageSize: candidatePool,
                ct);

        ICollection<TicketResponse> merged = MergeResults(semanticResult.Items, keywordResult.Items);

        ICollection<TicketResponse> paged = merged
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        PagedResult<TicketResponse> response = new()
        {
            Items = paged,
            TotalCount = merged.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<TicketResponse>>.Success(response);
    }
    #region private
    private static ICollection<TicketResponse> MergeResults(
        ICollection<Ticket> semantic,
        ICollection<Ticket> keyword)
    {
        const int k = 60;

        Dictionary<Guid, double> scores = new();

        int semanticRank = 1;

        foreach (Ticket ticket in semantic)
        {
            scores[ticket.Id] =
                scores.GetValueOrDefault(ticket.Id)
                + (1.0 / (k + semanticRank));

            semanticRank++;
        }

        int keywordRank = 1;

        foreach (Ticket ticket in keyword)
        {
            scores[ticket.Id] =
                scores.GetValueOrDefault(ticket.Id)
                + (1.0 / (k + keywordRank));

            keywordRank++;
        }

        return semantic
            .Concat(keyword)
            .GroupBy(x => x.Id)
            .Select(g =>
            {
                Ticket t = g.First();

                return new TicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Priority = t.Priority,
                    Status = t.Status
                };
            })
            .OrderByDescending(x => scores[x.Id])
            .ToList();
    }

    private static Result<PagedResult<TicketResponse>> Empty(SearchTicketsQuery request)
        => Result<PagedResult<TicketResponse>>.Success(new PagedResult<TicketResponse>
        {
            Items = [],
            TotalCount = 0,
            Page = request.Page,
            PageSize = request.PageSize
        });

    #endregion private
}