using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Comands.ResetPassword;

public sealed class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; init; } = default!;
    public string NewPassword { get; init; } = default!;
}
