using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Queries.GetUsersForAdmin;

public sealed class GetUsersForAdminQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersForAdminQuery, Result<GetUsersForAdminResponse>>
{
    private readonly IUserRepository _userRepository = userRepository
        ?? throw new ArgumentNullException(nameof(userRepository));

    public Task<Result<GetUsersForAdminResponse>> Handle(GetUsersForAdminQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<GetUsersForAdminResponse>> HandleInnerAsync([NotNull] GetUsersForAdminQuery request, CancellationToken ct)
    {
        ICollection<User> users = await _userRepository.GetUsersAsync(ct);

        ICollection<AdminUserDto> result = users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            AvatarUrl = u.ProfilePictureUrl,
            Role = u.UserRole
        }).ToList();

        return Result<GetUsersForAdminResponse>.Success(new GetUsersForAdminResponse(result));
    }
}
