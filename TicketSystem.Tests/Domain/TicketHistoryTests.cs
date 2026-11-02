using FluentAssertions;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Tests.Domain;

public class TicketHistoryTests
{
    private static Ticket CreateSavedTicket()
    {
        var ticket = new Ticket("Naslov", "Opis", createdBy: "Ana");
        ticket.ClearPendingHistory(); // simulate a ticket that was already persisted
        return ticket;
    }

    [Fact]
    public void Constructor_RecordsCreatedEntry()
    {
        var ticket = new Ticket("Naslov", "Opis", createdBy: "Ana");

        ticket.PendingHistory.Should().ContainSingle();
        TicketHistory entry = ticket.PendingHistory[0];
        entry.Field.Should().Be(TicketHistoryFields.Created);
        entry.ChangedBy.Should().Be("Ana");
        entry.TicketId.Should().Be(ticket.Id);
    }

    [Fact]
    public void ChangeStatus_RecordsEnumNames()
    {
        var ticket = CreateSavedTicket();

        ticket.ChangeStatus(TicketStatus.WaitingforCustomer, "Marko");

        TicketHistory entry = ticket.PendingHistory.Should().ContainSingle().Subject;
        entry.Field.Should().Be(TicketHistoryFields.Status);
        entry.OldValue.Should().Be("Open");
        entry.NewValue.Should().Be("WaitingforCustomer");
        entry.ChangedBy.Should().Be("Marko");
    }

    [Fact]
    public void ChangeStatus_SameValue_RecordsNothing()
    {
        var ticket = CreateSavedTicket();

        ticket.ChangeStatus(TicketStatus.Open, "Marko");

        ticket.PendingHistory.Should().BeEmpty();
    }

    [Fact]
    public void AssignTo_NewUser_RecordsAssigneeAndStatus()
    {
        var ticket = CreateSavedTicket();

        ticket.AssignTo(Guid.NewGuid(), "Petar Petrović", "Marko");

        ticket.PendingHistory.Select(h => h.Field)
            .Should().BeEquivalentTo([TicketHistoryFields.Assignee, TicketHistoryFields.Status]);
        ticket.PendingHistory.Single(h => h.Field == TicketHistoryFields.Assignee)
            .NewValue.Should().Be("Petar Petrović");
    }

    [Fact]
    public void AssignTo_ThenStatusBackToOpen_StatusChangesCancelOut()
    {
        var ticket = CreateSavedTicket();

        // Mirrors UpdateTicket: AssignTo forces InProgress, then the requested status is applied.
        ticket.AssignTo(Guid.NewGuid(), "Petar Petrović", "Marko");
        ticket.ChangeStatus(TicketStatus.Open, "Marko");

        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.PendingHistory.Should().ContainSingle()
            .Which.Field.Should().Be(TicketHistoryFields.Assignee);
    }

    [Fact]
    public void AssignTo_SameUser_RecordsNothingAndKeepsStatus()
    {
        var ticket = CreateSavedTicket();
        var userId = Guid.NewGuid();
        ticket.AssignTo(userId, "Petar Petrović", "Marko");
        ticket.ChangeStatus(TicketStatus.WaitingforCustomer, "Marko");
        ticket.ClearPendingHistory();

        ticket.AssignTo(userId, "Petar Petrović", "Marko");

        ticket.PendingHistory.Should().BeEmpty();
        ticket.Status.Should().Be(TicketStatus.WaitingforCustomer);
    }

    [Fact]
    public void ChangeStatus_TwiceBeforeSave_CollapsesIntoOneEntry()
    {
        var ticket = CreateSavedTicket();

        ticket.ChangeStatus(TicketStatus.InProgress, "Marko");
        ticket.ChangeStatus(TicketStatus.Closed, "Marko");

        TicketHistory entry = ticket.PendingHistory.Should().ContainSingle().Subject;
        entry.OldValue.Should().Be("Open");
        entry.NewValue.Should().Be("Closed");
    }

    [Fact]
    public void Update_OnlyDescriptionChanged_RecordsOnlyDescription()
    {
        var ticket = CreateSavedTicket();

        ticket.Update("Naslov", "Novi opis", "Marko");

        TicketHistory entry = ticket.PendingHistory.Should().ContainSingle().Subject;
        entry.Field.Should().Be(TicketHistoryFields.Description);
        entry.OldValue.Should().Be("Opis");
        entry.NewValue.Should().Be("Novi opis");
    }

    [Fact]
    public void Change_WithoutUser_FallsBackToSystem()
    {
        var ticket = CreateSavedTicket();

        ticket.ChangePriority(TicketPriority.Urgent);

        ticket.PendingHistory.Single().ChangedBy.Should().Be(TicketHistoryFields.SystemUser);
    }
}
