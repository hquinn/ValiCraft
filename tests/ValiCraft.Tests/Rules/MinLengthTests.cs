using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class MinLengthTests
{
    [Fact]
    public void IsValid_WhenStringMeetsMinLength_ReturnsTrue()
    {
        var result = MinLength.IsValid("hello", 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringExactlyMinLength_ReturnsTrue()
    {
        var result = MinLength.IsValid("hello", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringBelowMinLength_ReturnsFalse()
    {
        var result = MinLength.IsValid("hi", 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = MinLength.IsValid(null, 1);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenMinLengthIsZero_ReturnsTrue()
    {
        var result = MinLength.IsValid("", 0);
        result.Should().BeTrue();
    }
}
