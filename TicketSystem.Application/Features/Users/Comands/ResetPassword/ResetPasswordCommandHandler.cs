using Microsoft.AspNetCore.Identity;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.ResetPassword;

public sealed class ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private static readonly ErrorData InvalidOrExpiredToken = new("InvalidOrExpiredToken", "Link za reset lozinke nije važeći ili je istekao.");

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

    public Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<bool>> HandleInnerAsync([NotNull] ResetPasswordCommand request, CancellationToken ct)
    {
        string tokenHash = PasswordResetTokenHasher.Hash(request.Token);

        User? user = await _userRepository.GetUserByPasswordResetTokenHashAsync(tokenHash, ct);

        if (user is null || user.PasswordResetTokenExpiresAt is null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return Result<bool>.Fail(InvalidOrExpiredToken);
        }

        user.SetPassword(_passwordHasher.HashPassword(user, request.NewPassword));
        user.ClearPasswordResetToken();

        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
