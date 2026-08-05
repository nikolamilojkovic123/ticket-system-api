using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.UserManagment.Enums;

namespace TicketSystem.Domain.UserManagment.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? GoogleId { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public string? Expertise { get; set; }
    public List<Ticket> AssignedTickets { get; private set; } = new();
    public UserRole UserRole { get; private set; }
    public string? PasswordHash { get; private set; }
    public string? PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    private User() { }

    public User(string firstName, string lastName, string email, string? expertise, string? googleId = null, string? profilePictureUrl = null)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        GoogleId = googleId;
        ProfilePictureUrl = profilePictureUrl;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        PhoneNumber = string.Empty;
        Expertise = expertise;
        UserRole = UserRole.User;
    }

    public void SetPhone(string phone)
    {
        PhoneNumber = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? profilePictureUrl, string? expertise)
    {
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrl = profilePictureUrl;
        UpdatedAt = DateTime.UtcNow;
        Expertise = expertise;
    }

    public void UpdateGoogleInfo(string? googleId, string? profilePictureUrl)
    {
        GoogleId = googleId;
        ProfilePictureUrl = profilePictureUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public User SetUserRole(UserRole userRole)
    {
        UserRole = userRole;
        return this;
    }

    public void SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordResetToken(string tokenHash, DateTime expiresAt)
    {
        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenExpiresAt = expiresAt;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiresAt = null;
    }
}