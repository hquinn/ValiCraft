using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class MaxLengthTests
{
    [Fact]
    public void IsValid_WhenStringBelowMaxLength_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MaxLength("hi", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringExactlyMaxLength_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MaxLength("hello", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringExceedsMaxLength_ReturnsFalse()
    {
        var result = ValiCraft.Rules.MaxLength("hello world", 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsTrue()
    {
        // Null is treated as length 0, which passes MaxLength validation
        var result = ValiCraft.Rules.MaxLength(null, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEmptyStringAndMaxLengthZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MaxLength("", 0);
        result.Should().BeTrue();
    }
}
