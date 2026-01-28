using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotNullOrWhiteSpaceTests
{
    [Fact]
    public void IsValid_WhenStringHasValue_ReturnsTrue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        // Arrange
        string? value = null;

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsEmpty_ReturnsFalse()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsWhiteSpace_ReturnsFalse()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsTab_ReturnsFalse()
    {
        // Arrange
        var value = "\t";

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNewLine_ReturnsFalse()
    {
        // Arrange
        var value = "\n";

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringHasWhiteSpaceAndText_ReturnsTrue()
    {
        // Arrange
        var value = "  test  ";

        // Act
        var result = ValiCraft.Rules.NotNullOrWhiteSpace(value);

        // Assert
        result.Should().BeTrue();
    }
}
