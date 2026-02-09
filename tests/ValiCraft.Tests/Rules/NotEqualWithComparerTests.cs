using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotEqualWithComparerTests
{
    [Fact]
    public void IsValid_WhenValuesNotEqualWithComparer_ReturnsTrue()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = ValiCraft.Rules.NotEqual<string>("Hello", "world", comparer);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValuesEqualWithComparer_ReturnsFalse()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = ValiCraft.Rules.NotEqual<string>("Hello", "hello", comparer);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDefaultComparer_WorksCorrectly()
    {
        var comparer = EqualityComparer<int>.Default;
        var result = ValiCraft.Rules.NotEqual(42, 10, comparer);
        result.Should().BeTrue();
    }
}
