using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class PositiveTests
{
    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsTrue()
    {
        var result = Positive<int>.IsValid(5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsFalse()
    {
        var result = Positive<int>.IsValid(0);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsFalse()
    {
        var result = Positive<int>.IsValid(-5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = Positive<double>.IsValid(0.1);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNegativeDouble_ReturnsFalse()
    {
        var result = Positive<double>.IsValid(-0.1);
        result.Should().BeFalse();
    }
}
