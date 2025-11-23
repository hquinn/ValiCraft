using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class EmptyTests
{
    [Fact]
    public void IsValid_WhenCollectionIsEmpty_ReturnsTrue()
    {
        var result = Empty<int>.IsValid(Array.Empty<int>());
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsTrue()
    {
        var result = Empty<int>.IsValid(null);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionHasItems_ReturnsFalse()
    {
        var result = Empty<int>.IsValid(new[] { 1, 2, 3 });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithList_WorksCorrectly()
    {
        var result = Empty<string>.IsValid(new List<string>());
        result.Should().BeTrue();
    }
}
