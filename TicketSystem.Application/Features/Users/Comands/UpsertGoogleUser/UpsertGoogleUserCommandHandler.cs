using TicketSystem.Application.Mediator;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;

public sealed class UpsertGoogleUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpsertGoogleUserCommand, Guid>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    public Task<Guid> Handle(UpsertGoogleUserCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Guid> HandleInnerAsync([NotNull] UpsertGoogleUserCommand request, CancellationToken ct)
    {
        User? user = await _userRepository.GetUserByGoogleIdOrMailAsync(request.GoogleId, ct);

        if (user == null)
        {
            user = new User(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                expertise: request.Expertise,
                googleId: request.GoogleId,
                profilePictureUrl: request.ProfilePictureUrl
            );

            await userRepository.CreateUserAsync(user, ct);
        }
        else
        {
            user.UpdateProfile(
                firstName: request.FirstName,
                lastName: request.LastName,
                profilePictureUrl: request.ProfilePictureUrl,
                expertise: request.Expertise
            );

            user.UpdateGoogleInfo(
                googleId: request.GoogleId,
                profilePictureUrl: request.ProfilePictureUrl
            );

            await userRepository.UpdateAsync(user, ct);
        }

        return user.Id;
    }
}
