using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class MaxLengthTests
{
    [Fact]
    public void IsValid_WhenStringBelowMaxLength_ReturnsTrue()
    {
        var result = MaxLength.IsValid("hi", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringExactlyMaxLength_ReturnsTrue()
    {
        var result = MaxLength.IsValid("hello", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringExceedsMaxLength_ReturnsFalse()
    {
        var result = MaxLength.IsValid("hello world", 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = MaxLength.IsValid(null, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmptyStringAndMaxLengthZero_ReturnsTrue()
    {
        var result = MaxLength.IsValid("", 0);
        result.Should().BeTrue();
    }
}
