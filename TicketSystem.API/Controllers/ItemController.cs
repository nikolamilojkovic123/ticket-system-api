using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Tickets.Commands.CreateTicket;
using TicketSystem.Application.Mediator;

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
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest createTicketRequest, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(createTicketRequest, cancellationToken);
        return Ok(result);
    }
}
