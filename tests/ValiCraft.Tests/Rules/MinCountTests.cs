using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class MinCountTests
{
    [Fact]
    public void IsValid_WhenCollectionMeetsMinCount_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MinCount([1, 2, 3, 4, 5], 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionExactlyMinCount_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MinCount([1, 2, 3], 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionBelowMinCount_ReturnsFalse()
    {
        var result = ValiCraft.Rules.MinCount([1, 2], 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.MinCount<int>(null, 1);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenMinCountIsZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.MinCount(Array.Empty<int>(), 0);
        result.Should().BeTrue();
    }
}
