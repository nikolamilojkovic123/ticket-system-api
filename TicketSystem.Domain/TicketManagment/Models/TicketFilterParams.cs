using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Domain.TicketManagment.Models;

public sealed class TicketFilterParams
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public ICollection<TicketStatus> Statuses { get; init; } = [];
    public ICollection<TicketPriority> Priorities { get; init; } = [];
    public ICollection<TicketCategory> Categories { get; init; } = [];

    public Guid? AssigneeId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
