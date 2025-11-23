using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class LessThanTests
{
    [Fact]
    public void IsValid_WhenIntegerIsLess_ReturnsTrue()
    {
        // Arrange
        var value = 5;
        var comparison = 10;

        // Act
        var result = LessThan<int>.IsValid(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegerIsEqual_ReturnsFalse()
    {
        // Arrange
        var value = 10;
        var comparison = 10;

        // Act
        var result = LessThan<int>.IsValid(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenIntegerIsGreater_ReturnsFalse()
    {
        // Arrange
        var value = 10;
        var comparison = 5;

        // Act
        var result = LessThan<int>.IsValid(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDoubleIsLess_ReturnsTrue()
    {
        // Arrange
        var value = 3.3;
        var comparison = 5.5;

        // Act
        var result = LessThan<double>.IsValid(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDoubleIsGreater_ReturnsFalse()
    {
        // Arrange
        var value = 5.5;
        var comparison = 3.3;

        // Act
        var result = LessThan<double>.IsValid(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDateTimeIsLess_ReturnsTrue()
    {
        // Arrange
        var value = new DateTime(2025, 1, 10);
        var comparison = new DateTime(2025, 1, 15);

        // Act
        var result = LessThan<DateTime>.IsValid(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateTimeIsGreater_ReturnsFalse()
    {
        // Arrange
        var value = new DateTime(2025, 1, 15);
        var comparison = new DateTime(2025, 1, 10);

        // Act
        var result = LessThan<DateTime>.IsValid(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsLess_ReturnsTrue()
    {
        // Arrange
        var value = "apple";
        var comparison = "zebra";

        // Act
        var result = LessThan<string>.IsValid(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringIsGreater_ReturnsFalse()
    {
        // Arrange
        var value = "zebra";
        var comparison = "apple";

        // Act
        var result = LessThan<string>.IsValid(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNegativeIntegerIsLess_ReturnsTrue()
    {
        // Arrange
        var value = -10;
        var comparison = -5;

        // Act
        var result = LessThan<int>.IsValid(value, comparison);

        // Assert
        result.Should().BeTrue();
    }
}
