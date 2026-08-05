using FluentValidation;

namespace TicketSystem.Application.Features.Users.Comands.RegisterUser;

public sealed class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Unesite ispravnu email adresu");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Lozinka mora imati najmanje 8 karaktera");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Ime je obavezno");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Prezime je obavezno");
    }
}
