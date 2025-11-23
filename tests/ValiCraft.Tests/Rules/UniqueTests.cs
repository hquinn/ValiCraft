using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class UniqueTests
{
    [Fact]
    public void IsValid_WhenAllItemsUnique_ReturnsTrue()
    {
        var result = Unique<int>.IsValid(new[] { 1, 2, 3, 4, 5 });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDuplicateItemsExist_ReturnsFalse()
    {
        var result = Unique<int>.IsValid(new[] { 1, 2, 3, 2, 5 });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsTrue()
    {
        var result = Unique<int>.IsValid(null);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEmptyCollection_ReturnsTrue()
    {
        var result = Unique<int>.IsValid(Array.Empty<int>());
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = Unique<string>.IsValid(new[] { "apple", "banana", "cherry" });
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithDuplicateStrings_ReturnsFalse()
    {
        var result = Unique<string>.IsValid(new[] { "apple", "banana", "apple" });
        result.Should().BeFalse();
    }
}
