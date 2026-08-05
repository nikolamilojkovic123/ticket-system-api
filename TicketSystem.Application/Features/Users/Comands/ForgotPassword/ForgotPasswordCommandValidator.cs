using FluentValidation;

namespace TicketSystem.Application.Features.Users.Comands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Unesite ispravnu email adresu");
    }
}
