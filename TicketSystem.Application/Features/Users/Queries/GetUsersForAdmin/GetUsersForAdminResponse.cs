using TicketSystem.Application.Common.Collections;

namespace TicketSystem.Application.Features.Users.Queries.GetUsersForAdmin;

public sealed class GetUsersForAdminResponse(ICollection<AdminUserDto> items)
    : EntityCollectionResult<AdminUserDto>(items)
{
}
