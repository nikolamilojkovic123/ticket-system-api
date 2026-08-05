using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Comands.LoginUser;

public sealed class LoginUserCommand : IRequest<Result<AuthResult>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
