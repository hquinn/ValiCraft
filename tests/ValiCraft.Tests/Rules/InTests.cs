using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class InTests
{
    [Fact]
    public void IsValid_WhenValueIsInSet_ReturnsTrue()
    {
        var result = ValiCraft.Rules.In(3, [1, 2, 3, 4, 5]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsNotInSet_ReturnsFalse()
    {
        var result = ValiCraft.Rules.In(10, [1, 2, 3, 4, 5]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = ValiCraft.Rules.In<string>("banana", ["apple", "banana", "cherry"]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithStrings_IsCaseSensitive()
    {
        var result = ValiCraft.Rules.In<string>("BANANA", ["apple", "banana", "cherry"]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithEmptySet_ReturnsFalse()
    {
        var result = ValiCraft.Rules.In(1, []);
        result.Should().BeFalse();
    }
}
