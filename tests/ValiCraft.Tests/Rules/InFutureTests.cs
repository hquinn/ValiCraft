using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class InFutureTests
{
    [Fact]
    public void IsValid_WhenDateIsInFuture_ReturnsTrue()
    {
        var result = InFuture.IsValid(DateTime.UtcNow.AddDays(1));
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateIsInPast_ReturnsFalse()
    {
        var result = InFuture.IsValid(DateTime.UtcNow.AddDays(-1));
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDateIsTomorrow_ReturnsTrue()
    {
        var result = InFuture.IsValid(DateTime.UtcNow.AddHours(25));
        result.Should().BeTrue();
    }
}
