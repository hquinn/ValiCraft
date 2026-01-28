using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class EqualWithComparerTests
{
    [Fact]
    public void IsValid_WhenValuesEqualWithComparer_ReturnsTrue()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = ValiCraft.Rules.Equal<string>("Hello", "hello", comparer);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValuesNotEqualWithComparer_ReturnsFalse()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var result = ValiCraft.Rules.Equal<string>("Hello", "world", comparer);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDefaultComparer_WorksCorrectly()
    {
        var comparer = EqualityComparer<int>.Default;
        var result = ValiCraft.Rules.Equal(42, 42, comparer);
        result.Should().BeTrue();
    }
}
