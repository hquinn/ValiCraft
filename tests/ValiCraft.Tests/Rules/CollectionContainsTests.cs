using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class CollectionContainsTests
{
    [Fact]
    public void IsValid_WhenCollectionContainsItem_ReturnsTrue()
    {
        var result = ValiCraft.Rules.CollectionContains([1, 2, 3, 4, 5], 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionDoesNotContainItem_ReturnsFalse()
    {
        var result = ValiCraft.Rules.CollectionContains([1, 2, 3], 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.CollectionContains(null, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStringCollection_WorksCorrectly()
    {
        var result = ValiCraft.Rules.CollectionContains<string>(["apple", "banana", "cherry"], "banana");
        result.Should().BeTrue();
    }
}
