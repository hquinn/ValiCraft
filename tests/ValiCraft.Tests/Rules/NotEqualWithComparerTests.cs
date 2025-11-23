using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class NotEqualWithComparerTests
{
    [Fact]
    public void IsValid_WhenValuesNotEqualWithComparer_ReturnsTrue()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = NotEqualWithComparer<string>.IsValid("Hello", "world", comparer);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValuesEqualWithComparer_ReturnsFalse()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = NotEqualWithComparer<string>.IsValid("Hello", "hello", comparer);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDefaultComparer_WorksCorrectly()
    {
        var comparer = EqualityComparer<int>.Default;
        var result = NotEqualWithComparer<int>.IsValid(42, 10, comparer);
        result.Should().BeTrue();
    }
}
