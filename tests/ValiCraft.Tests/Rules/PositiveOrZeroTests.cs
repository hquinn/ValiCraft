using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class PositiveOrZeroTests
{
    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsTrue()
    {
        var result = ValiCraft.Rules.PositiveOrZero(5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.PositiveOrZero(0);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsFalse()
    {
        var result = ValiCraft.Rules.PositiveOrZero(-5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = ValiCraft.Rules.PositiveOrZero(0.0);
        result.Should().BeTrue();
    }
}
