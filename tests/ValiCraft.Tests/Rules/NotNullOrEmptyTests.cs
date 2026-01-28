using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotNullOrEmptyTests
{
    [Fact]
    public void IsValid_WhenStringHasValue_ReturnsTrue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = ValiCraft.Rules.NotNullOrEmpty(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        // Arrange
        string? value = null;

        // Act
        var result = ValiCraft.Rules.NotNullOrEmpty(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsEmpty_ReturnsFalse()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = ValiCraft.Rules.NotNullOrEmpty(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsWhiteSpace_ReturnsTrue()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = ValiCraft.Rules.NotNullOrEmpty(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringSingleCharacter_ReturnsTrue()
    {
        // Arrange
        var value = "a";

        // Act
        var result = ValiCraft.Rules.NotNullOrEmpty(value);

        // Assert
        result.Should().BeTrue();
    }
}
