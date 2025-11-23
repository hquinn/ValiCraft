using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class StartsWithTests
{
    [Fact]
    public void IsValid_WhenStringStartsWithPrefix_ReturnsTrue()
    {
        var result = StartsWith.IsValid("hello world", "hello");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotStartWithPrefix_ReturnsFalse()
    {
        var result = StartsWith.IsValid("hello world", "world");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = StartsWith.IsValid(null, "test");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenPrefixIsEmpty_ReturnsTrue()
    {
        var result = StartsWith.IsValid("hello", "");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_IsCaseSensitive_ReturnsFalse()
    {
        var result = StartsWith.IsValid("hello", "Hello");
        result.Should().BeFalse();
    }
}
