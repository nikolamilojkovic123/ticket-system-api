using TicketSystem.Application.Common.Collections;

namespace TicketSystem.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersResponse(ICollection<UserDto> items)
    : EntityCollectionResult<UserDto>(items)
{
}
