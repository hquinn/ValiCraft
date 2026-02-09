using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NegativeOrZeroTests
{
    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsTrue()
    {
        var result = ValiCraft.Rules.NegativeOrZero(-5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsTrue()
    {
        var result = ValiCraft.Rules.NegativeOrZero(0);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsFalse()
    {
        var result = ValiCraft.Rules.NegativeOrZero(5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = ValiCraft.Rules.NegativeOrZero(0.0);
        result.Should().BeTrue();
    }
}
