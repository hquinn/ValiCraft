using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class BetweenExclusiveTests
{
    [Fact]
    public void IsValid_WhenValueWithinRange_ReturnsTrue()
    {
        var result = BetweenExclusive<int>.IsValid(5, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueAtMinimum_ReturnsFalse()
    {
        var result = BetweenExclusive<int>.IsValid(1, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueAtMaximum_ReturnsFalse()
    {
        var result = BetweenExclusive<int>.IsValid(10, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueBelowMinimum_ReturnsFalse()
    {
        var result = BetweenExclusive<int>.IsValid(0, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueAboveMaximum_ReturnsFalse()
    {
        var result = BetweenExclusive<int>.IsValid(11, 1, 10);
        result.Should().BeFalse();
    }
}
