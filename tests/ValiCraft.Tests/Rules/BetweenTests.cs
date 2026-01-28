using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class BetweenTests
{
    [Fact]
    public void IsValid_WhenValueWithinRange_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Between(5, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueAtMinimum_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Between(1, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueAtMaximum_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Between(10, 1, 10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueBelowMinimum_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Between(0, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueAboveMaximum_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Between(11, 1, 10);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithDoubles_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Between(5.5, 1.0, 10.0);
        result.Should().BeTrue();
    }
}
