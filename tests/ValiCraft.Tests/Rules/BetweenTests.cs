using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class BetweenTests
{
    [Fact]
    public void IsValid_WhenValueWithinRange_ReturnsTrue()
    {
        var result = Between<int>.IsValid(5, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueAtMinimum_ReturnsTrue()
    {
        var result = Between<int>.IsValid(1, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueAtMaximum_ReturnsTrue()
    {
        var result = Between<int>.IsValid(10, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueBelowMinimum_ReturnsFalse()
    {
        var result = Between<int>.IsValid(0, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueAboveMaximum_ReturnsFalse()
    {
        var result = Between<int>.IsValid(11, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDoubles_WorksCorrectly()
    {
        var result = Between<double>.IsValid(5.5, 1.0, 10.0);
        result.Should().BeTrue();
    }
}
