namespace TicketSystem.Application.Features.Users.Queries.GetUsers;

public sealed class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? AvatarUrl { get; set; }
}
