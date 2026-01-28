using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class UniqueTests
{
    [Fact]
    public void IsValid_WhenAllItemsUnique_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Unique([1, 2, 3, 4, 5]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDuplicateItemsExist_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Unique([1, 2, 3, 2, 5]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Unique<int>(null);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEmptyCollection_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Unique(Array.Empty<int>());
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithStrings_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Unique<string>(["apple", "banana", "cherry"]);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithDuplicateStrings_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Unique<string>(["apple", "banana", "apple"]);
        result.Should().BeFalse();
    }
}
