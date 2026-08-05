using TicketSystem.Core;

namespace TicketSystem.Application.Common;

public static class ApplicationErrors
{
    public static readonly ErrorData NotFound = new("Application.NotFound", "The requested resource was not found.");
    public static readonly ErrorData Unauthorized =
        new("Application.Unauthorized", "Authentication is required to access this resource.");
    public static readonly ErrorData Forbidden =
        new("Application.Forbidden", "You do not have permission to perform this action.");

}
