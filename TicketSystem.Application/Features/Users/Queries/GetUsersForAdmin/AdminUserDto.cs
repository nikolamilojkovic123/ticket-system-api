using TicketSystem.Domain.UserManagment.Enums;

namespace TicketSystem.Application.Features.Users.Queries.GetUsersForAdmin;

public sealed class AdminUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
}
