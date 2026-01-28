using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class CountTests
{
    [Fact]
    public void IsValid_WhenCollectionHasExactCount_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Count([1, 2, 3], 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionCountDifferent_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Count([1, 2, 3], 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Count<int>(null, 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmptyCollectionAndCountZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Count(Array.Empty<int>(), 0);
        result.Should().BeTrue();
    }
}
