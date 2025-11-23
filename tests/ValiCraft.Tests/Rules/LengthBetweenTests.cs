using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class LengthBetweenTests
{
    [Fact]
    public void IsValid_WhenLengthWithinRange_ReturnsTrue()
    {
        var result = LengthBetween.IsValid("hello", 3, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthAtMinimum_ReturnsTrue()
    {
        var result = LengthBetween.IsValid("hello", 5, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthAtMaximum_ReturnsTrue()
    {
        var result = LengthBetween.IsValid("hello", 3, 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLengthBelowMinimum_ReturnsFalse()
    {
        var result = LengthBetween.IsValid("hi", 5, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenLengthAboveMaximum_ReturnsFalse()
    {
        var result = LengthBetween.IsValid("hello world", 1, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = LengthBetween.IsValid(null, 1, 10);
        result.Should().BeFalse();
    }
}
