using System.Security.Cryptography;
using System.Text;

namespace TicketSystem.Application.Features.Users.Common;

internal static class PasswordResetTokenHasher
{
    public static string GenerateToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public static string Hash(string token)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
