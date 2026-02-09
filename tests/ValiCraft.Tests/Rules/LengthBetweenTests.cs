using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class LengthBetweenTests
{
    [Fact]
    public void IsValid_WhenLengthWithinRange_ReturnsTrue()
    {
        var result = ValiCraft.Rules.LengthBetween("hello", 3, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthAtMinimum_ReturnsTrue()
    {
        var result = ValiCraft.Rules.LengthBetween("hello", 5, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthAtMaximum_ReturnsTrue()
    {
        var result = ValiCraft.Rules.LengthBetween("hello", 3, 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthBelowMinimum_ReturnsFalse()
    {
        var result = ValiCraft.Rules.LengthBetween("hi", 5, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenLengthAboveMaximum_ReturnsFalse()
    {
        var result = ValiCraft.Rules.LengthBetween("hello world", 1, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.LengthBetween(null, 1, 10);
        result.Should().BeFalse();
    }
}
