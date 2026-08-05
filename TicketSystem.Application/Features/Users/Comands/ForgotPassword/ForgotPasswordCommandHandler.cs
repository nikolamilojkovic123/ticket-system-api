using Microsoft.Extensions.Options;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Application.Services.Email;
using TicketSystem.Application.Services.Ui;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Comands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IEmailService emailService,
    IOptions<UiSettings> uiSettings)
    : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly UiSettings _uiSettings = uiSettings?.Value ?? throw new ArgumentNullException(nameof(uiSettings));

    public Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<bool>> HandleInnerAsync([NotNull] ForgotPasswordCommand request, CancellationToken ct)
    {
        User? user = await _userRepository.GetUserByEmailAsync(request.Email, ct);

        // Uvek vraćamo success bez obzira da li nalog postoji ili je Google-only —
        // sprečava otkrivanje da li je email registrovan (user enumeration).
        if (user is not null && user.PasswordHash is not null)
        {
            string rawToken = PasswordResetTokenHasher.GenerateToken();
            string tokenHash = PasswordResetTokenHasher.Hash(rawToken);

            user.SetPasswordResetToken(tokenHash, DateTime.UtcNow.Add(TokenLifetime));
            await _userRepository.UpdateAsync(user, ct);

            string resetLink = $"{_uiSettings.BaseUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(user.Email)}";
            await _emailService.SendPasswordResetAsync(user.Email, resetLink, ct);
        }

        return Result<bool>.Success(true);
    }
}
