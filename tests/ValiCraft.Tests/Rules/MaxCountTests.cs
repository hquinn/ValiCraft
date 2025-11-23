using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class MaxCountTests
{
    [Fact]
    public void IsValid_WhenCollectionBelowMaxCount_ReturnsTrue()
    {
        var result = MaxCount<int>.IsValid(new[] { 1, 2 }, 5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionExactlyMaxCount_ReturnsTrue()
    {
        var result = MaxCount<int>.IsValid(new[] { 1, 2, 3 }, 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCollectionExceedsMaxCount_ReturnsFalse()
    {
        var result = MaxCount<int>.IsValid(new[] { 1, 2, 3, 4, 5 }, 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = MaxCount<int>.IsValid(null, 10);
        result.Should().BeFalse();
    }
}
