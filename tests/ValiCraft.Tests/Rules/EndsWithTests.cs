using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class EndsWithTests
{
    [Fact]
    public void IsValid_WhenStringEndsWithSuffix_ReturnsTrue()
    {
        var result = ValiCraft.Rules.EndsWith("hello world", "world");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotEndWithSuffix_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EndsWith("hello world", "hello");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EndsWith(null, "test");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenSuffixIsEmpty_ReturnsTrue()
    {
        var result = ValiCraft.Rules.EndsWith("hello", "");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_IsCaseSensitive_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EndsWith("hello", "HELLO");
        result.Should().BeFalse();
    }
}
