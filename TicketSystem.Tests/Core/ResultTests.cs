using FluentAssertions;
using TicketSystem.Core;

namespace TicketSystem.Tests.Core;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResult()
    {
        var result = Result<string>.Success("test");

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("test");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Fail_WithError_ReturnsFailedResult()
    {
        var error = new Error("Test.Error", "Greška se desila");

        var result = Result<string>.Fail(error);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Code.Should().Be("Test.Error");
        result.Errors[0].Message.Should().Be("Greška se desila");
    }

    [Fact]
    public void Fail_WithMultipleErrors_ReturnsAllErrors()
    {
        var errors = new List<Error>
        {
            new("Error.One", "Prva greška"),
            new("Error.Two", "Druga greška")
        };

        var result = Result<string>.Fail(errors);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Success_NullData_ThrowsException()
    {
        var act = () => Result<string>.Success(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailedResult()
    {
        var error = new Error("Conv.Error", "Konverzija");

        Result<string> result = error;

        result.IsSuccess.Should().BeFalse();
        result.Errors[0].Code.Should().Be("Conv.Error");
    }
}
