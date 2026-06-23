using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.Application.Features.Users.Queries.UserProfile;

public sealed record UserProfileRequest(string email) :
    IRequest<Result<UserProfileResponse>>;
