using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class InTests
{
    [Fact]
    public void IsValid_WhenValueIsInSet_ReturnsTrue()
    {
        var result = In<int>.IsValid(3, new[] { 1, 2, 3, 4, 5 });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsNotInSet_ReturnsFalse()
    {
        var result = In<int>.IsValid(10, new[] { 1, 2, 3, 4, 5 });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = In<string>.IsValid("banana", new[] { "apple", "banana", "cherry" });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithStrings_IsCaseSensitive()
    {
        var result = In<string>.IsValid("BANANA", new[] { "apple", "banana", "cherry" });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithEmptySet_ReturnsFalse()
    {
        var result = In<int>.IsValid(1, Array.Empty<int>());
        result.Should().BeFalse();
    }
}
