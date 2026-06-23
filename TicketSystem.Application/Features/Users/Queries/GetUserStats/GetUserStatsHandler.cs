using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Application.Features.Users.Queries.GetUserStats;

public sealed class GetUserStatsHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserStatsQuery, Result<ICollection<UserStatsDto>>>
{
    private readonly IUserRepository _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task<Result<ICollection<UserStatsDto>>> Handle(GetUserStatsQuery request, CancellationToken ct)
    {
        ICollection<UserStats> stats = await _repo.GetUserStatsAsync(ct);

        ICollection<UserStatsDto> result = stats.Select(s => new UserStatsDto
        {
            UserId = s.UserId,
            Name = $"{s.FirstName} {s.LastName}",
            AvatarUrl = s.AvatarUrl,
            Total = s.Total,
            OpenCount = s.OpenCount,
            InProgressCount = s.InProgressCount,
            ClosedCount = s.ClosedCount,
            AvgResolutionDays = s.AvgResolutionDays,
        }).ToList();

        return Result<ICollection<UserStatsDto>>.Success(result);
    }
}
