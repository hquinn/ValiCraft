using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class GreaterOrEqualThanTests
{
    [Fact]
    public void IsValid_WhenIntegerIsGreater_ReturnsTrue()
    {
        // Arrange
        var value = 10;
        var comparison = 5;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsEqual_ReturnsTrue()
    {
        // Arrange
        var value = 10;
        var comparison = 10;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsLess_ReturnsFalse()
    {
        // Arrange
        var value = 5;
        var comparison = 10;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDoubleIsGreater_ReturnsTrue()
    {
        // Arrange
        var value = 5.5;
        var comparison = 3.3;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDoubleIsEqual_ReturnsTrue()
    {
        // Arrange
        var value = 3.14;
        var comparison = 3.14;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDoubleIsLess_ReturnsFalse()
    {
        // Arrange
        var value = 3.3;
        var comparison = 5.5;

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDateTimeIsGreater_ReturnsTrue()
    {
        // Arrange
        var value = new DateTime(2025, 1, 15);
        var comparison = new DateTime(2025, 1, 10);

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateTimeIsEqual_ReturnsTrue()
    {
        // Arrange
        var value = new DateTime(2025, 1, 15);
        var comparison = new DateTime(2025, 1, 15);

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateTimeIsLess_ReturnsFalse()
    {
        // Arrange
        var value = new DateTime(2025, 1, 10);
        var comparison = new DateTime(2025, 1, 15);

        // Act
        var result = ValiCraft.Rules.GreaterOrEqualThan(value, comparison);

        // Assert
        result.Should().BeFalse();
    }
}
