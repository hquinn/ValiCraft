using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NegativeTests
{
    [Fact]
    public void IsValid_WhenIntegerIsNegative_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Negative(-5);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsZero_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Negative(0);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenIntegerIsPositive_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Negative(5);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDouble_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Negative(-0.1);
        result.Should().BeTrue();
    }
}
