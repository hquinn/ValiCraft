using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class CollectionNotContainsTests
{
    [Fact]
    public void IsValid_WhenCollectionDoesNotContainItem_ReturnsTrue()
    {
        var result = ValiCraft.Rules.CollectionNotContains([1, 2, 3], 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionContainsItem_ReturnsFalse()
    {
        var result = ValiCraft.Rules.CollectionNotContains([1, 2, 3, 4, 5], 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsTrue()
    {
        var result = ValiCraft.Rules.CollectionNotContains(null, 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithStringCollection_WorksCorrectly()
    {
        var result = ValiCraft.Rules.CollectionNotContains<string>(["apple", "banana"], "cherry");
        result.Should().BeTrue();
    }
}
