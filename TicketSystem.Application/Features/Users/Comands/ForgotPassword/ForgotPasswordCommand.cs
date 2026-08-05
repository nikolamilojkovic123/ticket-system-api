using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Comands.ForgotPassword;

public sealed class ForgotPasswordCommand : IRequest<Result<bool>>
{
    public string Email { get; init; } = default!;
}
