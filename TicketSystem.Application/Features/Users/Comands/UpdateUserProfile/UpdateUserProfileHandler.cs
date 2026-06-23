using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Application.Features.Users.Comands.UpdateUserProfile;

public sealed class UpdateUserProfileHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserProfileRequest, Result<bool>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    public Task<Result<bool>> Handle(UpdateUserProfileRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<bool>> HandleInnerAsync(UpdateUserProfileRequest request, CancellationToken ct)
    {
        User? user = await _userRepository.GetUserByEmailAsync(request.Email, ct);

        if (user is null)
            return Result<bool>.Fail(Error.None);

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.ProfilePictureUrl,
            request.Expertise
           );

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.SetPhone(request.PhoneNumber);
        }
        await _userRepository.UpdateAsync(user, ct);

        return Result<bool>.Success(true);
    }
}
