using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.API.Common;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Features.Tickets.Commands.AddTicketComment;
using TicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Features.Tickets.Commands.UpdateTicket;
using TicketSystem.Application.Features.Tickets.Commands.UpdateTicketStatus;
using TicketSystem.Application.Features.Tickets.Dtos;
using TicketSystem.Application.Features.Tickets.Queries.GetAllTicketsQuery;
using TicketSystem.Application.Features.Tickets.Queries.GetByIdQuery;
using TicketSystem.Application.Features.Tickets.Queries.GetFilteredTickets;
using TicketSystem.Application.Features.Tickets.Queries.GetTicketComments;
using TicketSystem.Application.Features.Tickets.Queries.GetTicketHistory;
using TicketSystem.Application.Features.Tickets.Queries.SearchTickets;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/ticket")]
public class TicketController(SimpleMediator simpleMediator) : ControllerBase
{
    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));

    [HttpPost]
    public async Task<IActionResult> CreateTicketAsync([FromBody] CreateTicketRequest createTicketRequest, CancellationToken cancellationToken)
    {
        createTicketRequest.CreatedBy = CurrentUserName;
        Result<CreateTicketResponse> result = await _mediator.Send(createTicketRequest, cancellationToken);

        return result.ActionResult;
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<TicketResponse> result = await _mediator.Send(new GetTicketByIdQuery(id), cancellationToken);

        return result.ActionResult;
    }

    [HttpGet]
    public async Task<IActionResult> GetTicketsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ICollection<TicketStatus> statuses = null,
        [FromQuery] ICollection<TicketPriority> priorities = null,
        [FromQuery] ICollection<TicketCategory> categories = null,
        [FromQuery] Guid? assigneeId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        bool hasFilters = (statuses?.Count > 0) ||
                          (priorities?.Count > 0) ||
                          (categories?.Count > 0) ||
                          assigneeId.HasValue ||
                          dateFrom.HasValue ||
                          dateTo.HasValue;

        if (hasFilters)
        {
            PagedResult<TicketResponse> filtered = await _mediator.Send(
                new GetFilteredTicketsQuery(
                    page, pageSize,
                    statuses ?? [],
                    priorities ?? [],
                    categories ?? [],
                    assigneeId, dateFrom, dateTo),
                cancellationToken);

            return Ok(filtered);
        }

        PagedResult<TicketResponse> result = await _mediator.Send(new GetTicketsQuery(page, pageSize), cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTicketAsync(Guid id,
        [FromBody] CreateTicketRequest createTicketRequest, CancellationToken cancellationToken)
    {
        Result<UpdateTicketResponse> result = await _mediator.Send(new UpdateTicketCommand(id, createTicketRequest, CurrentUserName), cancellationToken);
        return result.ActionResult;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
       [FromQuery] string query,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10,
       CancellationToken cancellationToken = default)
    {
        Result<PagedResult<TicketResponse>> result =
            await _mediator.Send(
                new SearchTicketsQuery(
                    query,
                    page,
                    pageSize),
                cancellationToken);

        return result.ActionResult;
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddTicketCommentAsync(Guid id,
        [FromBody] AddCommentRequestBody body, CancellationToken cancellationToken)
    {
        string author = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        Result<TicketCommentResponse> result = await _mediator.Send(
            new AddTicketCommentCommand(id, author, body.Content), cancellationToken);

        return result.ActionResult;
    }

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetTicketCommentsAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<ICollection<TicketCommentResponse>> result = await _mediator.Send(
            new GetTicketCommentsQuery(id), cancellationToken);

        return result.ActionResult;
    }
    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetTicketHistoryAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<ICollection<TicketHistoryResponse>> result = await _mediator.Send(
            new GetTicketHistoryQuery(id), cancellationToken);

        return result.ActionResult;
    }

    private string CurrentUserName =>
        User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";

    [HttpPut("update-status/{id:guid}")]
    public async Task<IActionResult> UpdateTicketStatusAsync(Guid id,
        [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        Result<UpdateTicketStatusResponse> result = await _mediator.Send(
            new UpdateTicketStatusCommand(id, request.Status, CurrentUserName), cancellationToken);
        return result.ActionResult;
    }
}


