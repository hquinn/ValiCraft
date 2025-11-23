using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class PositiveOrZeroTests
{
    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsTrue()
    {
        var result = PositiveOrZero<int>.IsValid(5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsTrue()
    {
        var result = PositiveOrZero<int>.IsValid(0);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsFalse()
    {
        var result = PositiveOrZero<int>.IsValid(-5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = PositiveOrZero<double>.IsValid(0.0);
        result.Should().BeTrue();
    }
}
