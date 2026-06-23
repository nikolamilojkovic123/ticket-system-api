using TicketSystem.Application.Mediator;

namespace TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;

public sealed class UpsertGoogleUserCommand : IRequest<Guid>
{
    public string Email { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string GoogleId { get; init; } = default!;
    public string? Expertise { get; set; }
    public string ProfilePictureUrl { get; init; } = default!;
}
