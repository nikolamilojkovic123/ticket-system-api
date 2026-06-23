using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Chat;
using TicketSystem.Application.Mediator;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController(SimpleMediator simpleMediator) : ControllerBase
{

    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] GenerateAiResponseCommand command)
    {
        AiResponseDto result = await _mediator.Send(command);

        return Ok(new
        {
            response = result
        });
    }
}
