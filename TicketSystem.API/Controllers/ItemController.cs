using Microsoft.AspNetCore.Mvc;
using TicketSystem.API.Common;
using TicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/item")]
public class ItemController : ControllerBase
{
    private readonly SimpleMediator _mediator;

    public ItemController(SimpleMediator simpleMediator)
    {
        _mediator = simpleMediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest createTicketRequest, CancellationToken cancellationToken)
    {
        Result<CreateTicketResponse> result = await _mediator.Send(createTicketRequest, cancellationToken);
        return result.ActionResult;
    }
}
