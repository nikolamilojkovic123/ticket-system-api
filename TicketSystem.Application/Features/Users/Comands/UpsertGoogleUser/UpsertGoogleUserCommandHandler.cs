using Microsoft.Extensions.Options;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.Admin;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Enums;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;

public sealed class UpsertGoogleUserCommandHandler(IUserRepository userRepository, IOptions<AdminSettings> adminSettings)
    : IRequestHandler<UpsertGoogleUserCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly AdminSettings _adminSettings = adminSettings?.Value ?? throw new ArgumentNullException(nameof(adminSettings));

    public Task<Result<AuthResult>> Handle(UpsertGoogleUserCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<AuthResult>> HandleInnerAsync([NotNull] UpsertGoogleUserCommand request, CancellationToken ct)
    {
        User? userByGoogleId = string.IsNullOrEmpty(request.GoogleId)
            ? null
            : await _userRepository.GetUserByGoogleIdOrMailAsync(request.GoogleId, ct);

        User? user = userByGoogleId ?? await _userRepository.GetUserByEmailAsync(request.Email, ct);
        bool matchedByEmailOnly = userByGoogleId is null && user is not null;

        if (matchedByEmailOnly && user!.PasswordHash is not null && user.GoogleId is null)
        {
            return Result<AuthResult>.Fail(new ErrorData(
                "EmailTakenByPasswordAccount",
                "Ovaj email je već registrovan sa lozinkom. Prijavite se preko forme za login."));
        }

        bool isConfiguredAdmin = !string.IsNullOrWhiteSpace(_adminSettings.AdminEmail) &&
            request.Email.Equals(_adminSettings.AdminEmail, StringComparison.OrdinalIgnoreCase);

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

            user.SetUserRole(isConfiguredAdmin ? UserRole.Admin : UserRole.User);

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

            // Nikad ne oduzima Admin rolu automatski; samo je dodeljuje configured admin nalogu
            // ili popravlja legacy/nedodeljenu (0) rolu na default User.
            if (isConfiguredAdmin && user.UserRole != UserRole.Admin)
            {
                user.SetUserRole(UserRole.Admin);
            }
            else if (!isConfiguredAdmin && user.UserRole == default)
            {
                user.SetUserRole(UserRole.User);
            }

            await userRepository.UpdateAsync(user, ct);
        }

        return Result<AuthResult>.Success(new AuthResult(user.Id, user.Email, user.FullName, user.UserRole));
    }
}
