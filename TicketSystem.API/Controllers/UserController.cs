using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.API.Common;
using TicketSystem.Application.Features.Users.Comands.UpdateUserProfile;
using TicketSystem.Application.Features.Users.Comands.UpdateUserRole;
using TicketSystem.Application.Features.Users.Queries.GetUsers;
using TicketSystem.Application.Features.Users.Queries.GetUsersForAdmin;
using TicketSystem.Application.Features.Users.Queries.GetUserStats;
using TicketSystem.Application.Features.Users.Queries.UserProfile;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

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
        return response.ActionResult;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetUserStats(CancellationToken cancellationToken)
    {
        Result<ICollection<UserStatsDto>> response = await _mediator.Send(new GetUserStatsQuery(), cancellationToken);
        return response.ActionResult;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        Result<GetUsersResponse> response = await _mediator.Send(new GetUsersQuery(), cancellationToken);
        return response.ActionResult;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        request.Email = email;

        Result<UpdateUserProfileResponse> response = await _mediator.Send(request, cancellationToken);
        return response.ActionResult;
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersForAdmin(CancellationToken cancellationToken)
    {
        Result<GetUsersForAdminResponse> response = await _mediator.Send(new GetUsersForAdminQuery(), cancellationToken);
        return response.ActionResult;
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        request.UserId = id;

        Result<UpdateUserRoleResponse> response = await _mediator.Send(request, cancellationToken);
        return response.ActionResult;
    }
}
