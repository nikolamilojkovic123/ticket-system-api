using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Users.Comands.UpdateUserProfile;
using TicketSystem.Application.Features.Users.Queries.GetUsers;
using TicketSystem.Application.Features.Users.Queries.GetUserStats;
using TicketSystem.Application.Features.Users.Queries.UserProfile;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using System.Security.Claims;

namespace TicketSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/user")]
public class UserController(SimpleMediator simpleMediator) : ControllerBase
{
    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        Result<UserProfileResponse> response = await _mediator.Send(new UserProfileRequest(email), cancellationToken);
        return Ok(response);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStats(CancellationToken cancellationToken)
    {
        Result<ICollection<UserStatsDto>> response = await _mediator.Send(new GetUserStatsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        Result<GetUsersResponse> response = await _mediator.Send(new GetUsersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        request.Email = email;

        Result<bool> response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
