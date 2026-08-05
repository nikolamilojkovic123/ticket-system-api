using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Comands.RegisterUser;

public sealed class RegisterUserCommand : IRequest<Result<AuthResult>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
}
