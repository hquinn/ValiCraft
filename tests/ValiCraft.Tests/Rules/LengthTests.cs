using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class LengthTests
{
    [Fact]
    public void IsValid_WhenStringHasExactLength_ReturnsTrue()
    {
        var result = Length.IsValid("hello", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringLengthDifferent_ReturnsFalse()
    {
        var result = Length.IsValid("hello", 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = Length.IsValid(null, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmptyStringAndLengthZero_ReturnsTrue()
    {
        var result = Length.IsValid("", 0);
        result.Should().BeTrue();
    }
}
