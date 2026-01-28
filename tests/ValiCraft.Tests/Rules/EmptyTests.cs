using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class EmptyTests
{
    [Fact]
    public void IsValid_WhenCollectionIsEmpty_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Empty(Array.Empty<int>());
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Empty<int>(null);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionHasItems_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Empty([1, 2, 3]);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithList_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Empty<string>(new List<string>());
        result.Should().BeTrue();
    }
}
