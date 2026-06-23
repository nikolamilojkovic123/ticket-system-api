using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Application.Features.Users.Queries.UserProfile;

public class UserProfileHandler(IUserRepository userRepository)
    : IRequestHandler<UserProfileRequest, Result<UserProfileResponse>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public Task<Result<UserProfileResponse>> Handle(UserProfileRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        return HandleInnerAsync(request, ct);
    }

    private async Task<Result<UserProfileResponse>> HandleInnerAsync([NotNull] UserProfileRequest request, CancellationToken ct)
    {

        User? user = await _userRepository.GetUserByEmailAsync(request.email, ct);

        if (user is null)
            return Result<UserProfileResponse>.Fail(Error.None);

        UserProfileResponse result = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Expertise = user.Expertise
        };

        return Result<UserProfileResponse>.Success(result);
    }
}

