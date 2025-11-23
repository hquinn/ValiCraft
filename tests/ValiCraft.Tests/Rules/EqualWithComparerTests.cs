using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class EqualWithComparerTests
{
    [Fact]
    public void IsValid_WhenValuesEqualWithComparer_ReturnsTrue()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = EqualWithComparer<string>.IsValid("Hello", "hello", comparer);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValuesNotEqualWithComparer_ReturnsFalse()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = EqualWithComparer<string>.IsValid("Hello", "world", comparer);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDefaultComparer_WorksCorrectly()
    {
        var comparer = EqualityComparer<int>.Default;
        var result = EqualWithComparer<int>.IsValid(42, 42, comparer);
        result.Should().BeTrue();
    }
}
