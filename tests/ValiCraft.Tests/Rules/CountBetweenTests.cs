using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class CountBetweenTests
{
    [Fact]
    public void IsValid_WhenCountWithinRange_ReturnsTrue()
    {
        var result = CountBetween<int>.IsValid(new[] { 1, 2, 3, 4, 5 }, 3, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCountAtMinimum_ReturnsTrue()
    {
        var result = CountBetween<int>.IsValid(new[] { 1, 2, 3 }, 3, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCountAtMaximum_ReturnsTrue()
    {
        var result = CountBetween<int>.IsValid(new[] { 1, 2, 3 }, 1, 3);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenCountBelowMinimum_ReturnsFalse()
    {
        var result = CountBetween<int>.IsValid(new[] { 1, 2 }, 5, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCountAboveMaximum_ReturnsFalse()
    {
        var result = CountBetween<int>.IsValid(new[] { 1, 2, 3, 4, 5 }, 1, 3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenCollectionIsNull_ReturnsFalse()
    {
        var result = CountBetween<int>.IsValid(null, 1, 10);
        result.Should().BeFalse();
    }
}
