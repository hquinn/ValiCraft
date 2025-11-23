using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class ContainsTests
{
    [Fact]
    public void IsValid_WhenStringContainsSubstring_ReturnsTrue()
    {
        var result = Contains.IsValid("hello world", "lo wo");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotContainSubstring_ReturnsFalse()
    {
        var result = Contains.IsValid("hello world", "xyz");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = Contains.IsValid(null, "test");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenSubstringIsEmpty_ReturnsTrue()
    {
        var result = Contains.IsValid("hello", "");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_IsCaseSensitive_ReturnsFalse()
    {
        var result = Contains.IsValid("hello", "HELLO");
        result.Should().BeFalse();
    }
}
