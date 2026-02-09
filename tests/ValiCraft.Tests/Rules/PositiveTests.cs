using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class PositiveTests
{
    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Positive(5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Positive(0);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Positive(-5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Positive(0.1);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNegativeDouble_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Positive(-0.1);
        result.Should().BeFalse();
    }
}
