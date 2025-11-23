using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class CollectionContainsTests
{
    [Fact]
    public void IsValid_WhenCollectionContainsItem_ReturnsTrue()
    {
        var result = CollectionContains<int>.IsValid(new[] { 1, 2, 3, 4, 5 }, 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionDoesNotContainItem_ReturnsFalse()
    {
        var result = CollectionContains<int>.IsValid(new[] { 1, 2, 3 }, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = CollectionContains<int>.IsValid(null, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithStringCollection_WorksCorrectly()
    {
        var result = CollectionContains<string>.IsValid(new[] { "apple", "banana", "cherry" }, "banana");
        result.Should().BeTrue();
    }
}
