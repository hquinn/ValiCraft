using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class NegativeOrZeroTests
{
    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsTrue()
    {
        var result = NegativeOrZero<int>.IsValid(-5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsTrue()
    {
        var result = NegativeOrZero<int>.IsValid(0);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsFalse()
    {
        var result = NegativeOrZero<int>.IsValid(5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = NegativeOrZero<double>.IsValid(0.0);
        result.Should().BeTrue();
    }
}
