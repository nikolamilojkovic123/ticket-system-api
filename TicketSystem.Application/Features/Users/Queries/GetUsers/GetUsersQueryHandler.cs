using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, Result<GetUsersResponse>>
{
    private readonly IUserRepository _userRepository = userRepository
        ?? throw new ArgumentNullException(nameof(userRepository));
    public Task<Result<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<GetUsersResponse>> HandleInnerAsync([NotNull] GetUsersQuery request, CancellationToken ct)
    {
        ICollection<User> users = await _userRepository.GetUsersAsync(ct);

        ICollection<UserDto> result = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.FullName,
            AvatarUrl = u.ProfilePictureUrl
        }).ToList();

        return Result<GetUsersResponse>.Success(new GetUsersResponse(result));
    }
}
