using FluentValidation;

namespace TicketSystem.Application.Features.Users.Comands.LoginUser;

public sealed class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Unesite ispravnu email adresu");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Lozinka je obavezna");
    }
}
