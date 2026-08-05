using Microsoft.AspNetCore.Identity;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.LoginUser;

public sealed class LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    : IRequestHandler<LoginUserCommand, Result<AuthResult>>
{
    private static readonly ErrorData InvalidCredentials = new("InvalidCredentials", "Pogrešan email ili lozinka.");

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

    public Task<Result<AuthResult>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<AuthResult>> HandleInnerAsync([NotNull] LoginUserCommand request, CancellationToken ct)
    {
        User? user = await _userRepository.GetUserByEmailAsync(request.Email, ct);

        if (user is null || user.PasswordHash is null)
        {
            return Result<AuthResult>.Fail(InvalidCredentials);
        }

        PasswordVerificationResult verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            return Result<AuthResult>.Fail(InvalidCredentials);
        }

        return Result<AuthResult>.Success(new AuthResult(user.Id, user.Email, user.FullName, user.UserRole));
    }
}
