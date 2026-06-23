using TicketSystem.Domain.UserManagment.Entities;

namespace TicketSystem.Domain.UserManagment.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByGoogleIdOrMailAsync(string googleIdOrMail, CancellationToken cancellationToken);
    Task CreateUserAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ICollection<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task<List<UserStats>> GetUserStatsAsync(CancellationToken ct);
}
