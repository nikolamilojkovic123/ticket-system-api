using FluentValidation;

namespace TicketSystem.Application.Features.Users.Comands.ResetPassword;

public sealed class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token je obavezan");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Lozinka mora imati najmanje 8 karaktera");
    }
}
