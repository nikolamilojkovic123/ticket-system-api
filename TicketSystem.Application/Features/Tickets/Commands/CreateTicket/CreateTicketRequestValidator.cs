using FluentValidation;

namespace TicketSystem.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketRequestValidator
    : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title cannot be null or empty");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description cannot be null or empty");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid ticket category");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid ticket priority");
    }
}
