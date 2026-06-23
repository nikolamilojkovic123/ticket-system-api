using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Dashboard.Queries;
using TicketSystem.Application.Mediator;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(SimpleMediator simpleMediator) : Controller
{
    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        return Ok(await _mediator.Send(new GetDashboardQuery()));
    }
}
