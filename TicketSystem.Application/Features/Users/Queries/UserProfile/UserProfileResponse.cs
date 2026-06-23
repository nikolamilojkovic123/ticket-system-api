namespace TicketSystem.Application.Features.Users.Queries.UserProfile;

public sealed class UserProfileResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Expertise { get; set; }
}
