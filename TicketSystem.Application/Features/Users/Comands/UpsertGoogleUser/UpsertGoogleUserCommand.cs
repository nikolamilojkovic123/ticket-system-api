using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;

public sealed class UpsertGoogleUserCommand : IRequest<Result<AuthResult>>
{
    public string Email { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string GoogleId { get; init; } = default!;
    public string? Expertise { get; set; }
    public string ProfilePictureUrl { get; init; } = default!;
}
