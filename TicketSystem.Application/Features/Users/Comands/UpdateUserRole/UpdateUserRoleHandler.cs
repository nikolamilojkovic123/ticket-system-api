using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Application.Features.Users.Comands.UpdateUserRole;

public sealed class UpdateUserRoleHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserRoleCommand, Result<UpdateUserRoleResponse>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public Task<Result<UpdateUserRoleResponse>> Handle(UpdateUserRoleCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<UpdateUserRoleResponse>> HandleInnerAsync(UpdateUserRoleCommand request, CancellationToken ct)
    {
        User? user = await _userRepository.GetUserByIdAsync(request.UserId, ct);

        if (user is null)
            return Result<UpdateUserRoleResponse>.Fail(ErrorData.None);

        user.SetUserRole(request.Role);

        await _userRepository.UpdateAsync(user, ct);

        return Result<UpdateUserRoleResponse>.Success(new()
        {
            IsSuccess = true,
        });
    }
}
