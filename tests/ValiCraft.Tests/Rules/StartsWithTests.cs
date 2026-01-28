using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class StartsWithTests
{
    [Fact]
    public void IsValid_WhenStringStartsWithPrefix_ReturnsTrue()
    {
        var result = ValiCraft.Rules.StartsWith("hello world", "hello");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotStartWithPrefix_ReturnsFalse()
    {
        var result = ValiCraft.Rules.StartsWith("hello world", "world");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.StartsWith(null, "test");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenPrefixIsEmpty_ReturnsTrue()
    {
        var result = ValiCraft.Rules.StartsWith("hello", "");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_IsCaseSensitive_ReturnsFalse()
    {
        var result = ValiCraft.Rules.StartsWith("hello", "Hello");
        result.Should().BeFalse();
    }
}
