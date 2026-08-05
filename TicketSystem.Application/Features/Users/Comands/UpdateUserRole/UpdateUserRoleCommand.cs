using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Enums;

namespace TicketSystem.Application.Features.Users.Comands.UpdateUserRole;

public sealed class UpdateUserRoleCommand
    : IRequest<Result<UpdateUserRoleResponse>>
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
}
