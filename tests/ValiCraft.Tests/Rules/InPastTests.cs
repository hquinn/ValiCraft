using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class InPastTests
{
    [Fact]
    public void IsValid_WhenDateIsInPast_ReturnsTrue()
    {
        var result = ValiCraft.Rules.InPast(DateTime.UtcNow.AddDays(-1));
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateIsInFuture_ReturnsFalse()
    {
        var result = ValiCraft.Rules.InPast(DateTime.UtcNow.AddDays(1));
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDateIsYesterday_ReturnsTrue()
    {
        var result = ValiCraft.Rules.InPast(DateTime.UtcNow.AddHours(-25));
        result.Should().BeTrue();
    }
}
