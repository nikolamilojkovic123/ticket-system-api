using FluentAssertions;
using TicketSystem.Domain.TicketManagment.Entities;
using TicketSystem.Domain.TicketManagment.Enums;

namespace TicketSystem.Tests.Domain;

public class TicketTests
{
    [Fact]
    public void Constructor_ValidData_CreatesTicket()
    {
        var ticket = new Ticket("Bug u loginu", "Korisnik ne može da se uloguje");

        ticket.Id.Should().NotBeEmpty();
        ticket.Title.Should().Be("Bug u loginu");
        ticket.Description.Should().Be("Korisnik ne može da se uloguje");
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.Priority.Should().Be(TicketPriority.Low);
        ticket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyTitle_ThrowsException(string title)
    {
        var act = () => new Ticket(title, "Opis");

        act.Should().Throw<ArgumentException>().WithMessage("*Title*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyDescription_ThrowsException(string description)
    {
        var act = () => new Ticket("Naslov", description);

        act.Should().Throw<ArgumentException>().WithMessage("*Description*");
    }

    [Fact]
    public void AssignTo_ValidUserId_ChangesStatusToInProgress()
    {
        var ticket = new Ticket("Naslov", "Opis");
        var userId = Guid.NewGuid();

        ticket.AssignTo(userId);

        ticket.AssigneeId.Should().Be(userId);
        ticket.Status.Should().Be(TicketStatus.InProgress);
    }

    [Fact]
    public void ChangeStatus_ValidStatus_UpdatesStatus()
    {
        var ticket = new Ticket("Naslov", "Opis");

        ticket.ChangeStatus(TicketStatus.Closed);

        ticket.Status.Should().Be(TicketStatus.Closed);
        ticket.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ChangePriority_ValidPriority_UpdatesPriority()
    {
        var ticket = new Ticket("Naslov", "Opis");

        ticket.ChangePriority(TicketPriority.High);

        ticket.Priority.Should().Be(TicketPriority.High);
    }

    [Fact]
    public void ApplyAiAnalysis_ValidData_SetsAiFields()
    {
        var ticket = new Ticket("Naslov", "Opis");

        ticket.ApplyAiAnalysis("AI rezime", new[] { "bug", "login" }, 8.5);

        ticket.AiSummary.Should().Be("AI rezime");
        ticket.Keywords.Should().Contain("bug");
        ticket.Keywords.Should().Contain("login");
        ticket.SeverityScore.Should().Be(8.5);
    }

    [Fact]
    public void ApplyAiAnalysis_EmptySummary_ThrowsException()
    {
        var ticket = new Ticket("Naslov", "Opis");

        var act = () => ticket.ApplyAiAnalysis("", null, 5.0);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SetEmbedding_StoresAndRetrievesCorrectly()
    {
        var ticket = new Ticket("Naslov", "Opis");
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };

        ticket.SetEmbedding(embedding);
        var retrieved = ticket.GetEmbedding();

        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(embedding);
    }

    [Fact]
    public void GetEmbedding_NoEmbeddingSet_ReturnsNull()
    {
        var ticket = new Ticket("Naslov", "Opis");

        ticket.GetEmbedding().Should().BeNull();
    }
}
