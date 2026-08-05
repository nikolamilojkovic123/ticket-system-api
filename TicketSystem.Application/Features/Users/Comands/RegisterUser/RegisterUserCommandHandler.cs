using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.Admin;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Enums;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IOptions<AdminSettings> adminSettings)
    : IRequestHandler<RegisterUserCommand, Result<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    private readonly AdminSettings _adminSettings = adminSettings?.Value ?? throw new ArgumentNullException(nameof(adminSettings));

    public Task<Result<AuthResult>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<AuthResult>> HandleInnerAsync([NotNull] RegisterUserCommand request, CancellationToken ct)
    {
        User? existing = await _userRepository.GetUserByEmailAsync(request.Email, ct);

        if (existing is not null)
        {
            return Result<AuthResult>.Fail(new ErrorData(
                "EmailAlreadyRegistered",
                "Nalog sa ovim mejlom već postoji. Ako ste se ranije prijavljivali preko Google-a, koristite Google login."));
        }

        bool isConfiguredAdmin = !string.IsNullOrWhiteSpace(_adminSettings.AdminEmail) &&
            request.Email.Equals(_adminSettings.AdminEmail, StringComparison.OrdinalIgnoreCase);

        User user = new(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            expertise: null
        );

        user.SetUserRole(isConfiguredAdmin ? UserRole.Admin : UserRole.User);
        user.SetPassword(_passwordHasher.HashPassword(user, request.Password));

        await _userRepository.CreateUserAsync(user, ct);

        return Result<AuthResult>.Success(new AuthResult(user.Id, user.Email, user.FullName, user.UserRole));
    }
}
