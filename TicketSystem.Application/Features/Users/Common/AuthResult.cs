using TicketSystem.Domain.UserManagment.Enums;

namespace TicketSystem.Application.Features.Users.Common;

public sealed record AuthResult(Guid UserId, string Email, string FullName, UserRole Role);
