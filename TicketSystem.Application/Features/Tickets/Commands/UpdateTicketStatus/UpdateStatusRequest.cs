using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Application.Features.Tickets.Commands.UpdateTicketStatus;

public record UpdateStatusRequest(TicketStatus Status);