using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.TicketManagment.Enums;
using TicketSystem.Domain.UserManagment;
using TicketSystem.Domain.UserManagment.Entities;
using TicketSystem.Domain.UserManagment.Repositories;

namespace TicketSystem.Infrastructure.Database.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext)
    : IUserRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    =>
        await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<User?> GetUserByGoogleIdOrMailAsync(string googleIdOrMail, CancellationToken cancellationToken)
    =>
    await _dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == googleIdOrMail || u.Email == googleIdOrMail, cancellationToken);

    public async Task<ICollection<User>> GetUsers(CancellationToken cancellationToken)
    =>
        await _dbContext
        .Users
        .ToListAsync(cancellationToken);

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<User>> GetUsersAsync(CancellationToken cancellationToken)
    =>
        await _dbContext.Users.ToListAsync(cancellationToken);

    public async Task<List<UserStats>> GetUserStatsAsync(CancellationToken ct)
    {
        List<User> users = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.AssignedTickets)
            .ToListAsync(ct);

        return users.Select(u =>
        {
            List<double> durations = u.AssignedTickets
                .Where(t => t.Status == TicketStatus.Closed && t.UpdatedAt > t.CreatedAt)
                .Select(t => (t.UpdatedAt - t.CreatedAt).TotalDays)
                .ToList();

            return new UserStats
            {
                UserId           = u.Id,
                FirstName        = u.FirstName,
                LastName         = u.LastName,
                AvatarUrl        = u.ProfilePictureUrl,
                Total            = u.AssignedTickets.Count,
                OpenCount        = u.AssignedTickets.Count(t => t.Status == TicketStatus.Open),
                InProgressCount  = u.AssignedTickets.Count(t => t.Status == TicketStatus.InProgress),
                ClosedCount      = u.AssignedTickets.Count(t => t.Status == TicketStatus.Closed),
                AvgResolutionDays = durations.Count > 0 ? Math.Round(durations.Average(), 1) : null,
            };
        })
        .OrderByDescending(u => u.Total)
        .ToList();
    }
}
