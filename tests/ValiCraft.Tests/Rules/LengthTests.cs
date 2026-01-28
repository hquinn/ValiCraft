using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class LengthTests
{
    [Fact]
    public void IsValid_WhenStringHasExactLength_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Length("hello", 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringLengthDifferent_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Length("hello", 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Length(null, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmptyStringAndLengthZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Length("", 0);
        result.Should().BeTrue();
    }
}
