using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotInTests
{
    [Fact]
    public void IsValid_WhenValueIsNotInSet_ReturnsTrue()
    {
        var result = ValiCraft.Rules.NotIn(10, [1, 2, 3, 4, 5]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsInSet_ReturnsFalse()
    {
        var result = ValiCraft.Rules.NotIn(3, [1, 2, 3, 4, 5]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = ValiCraft.Rules.NotIn<string>("grape", ["apple", "banana", "cherry"]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptySet_ReturnsTrue()
    {
        var result = ValiCraft.Rules.NotIn(1, []);
        result.Should().BeTrue();
    }
}
