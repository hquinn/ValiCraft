using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class NotInTests
{
    [Fact]
    public void IsValid_WhenValueIsNotInSet_ReturnsTrue()
    {
        var result = NotIn<int>.IsValid(10, new[] { 1, 2, 3, 4, 5 });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsInSet_ReturnsFalse()
    {
        var result = NotIn<int>.IsValid(3, new[] { 1, 2, 3, 4, 5 });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = NotIn<string>.IsValid("grape", new[] { "apple", "banana", "cherry" });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptySet_ReturnsTrue()
    {
        var result = NotIn<int>.IsValid(1, Array.Empty<int>());
        result.Should().BeTrue();
    }
}
