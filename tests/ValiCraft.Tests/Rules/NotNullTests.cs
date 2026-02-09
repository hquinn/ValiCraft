using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotNullTests
{
    [Fact]
    public void IsValid_WhenValueIsNotNull_ReturnsTrue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = ValiCraft.Rules.NotNull<string>(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsNull_ReturnsFalse()
    {
        // Arrange
        string? value = null;

        // Act
        var result = ValiCraft.Rules.NotNull<string>(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenReferenceTypeIsNotNull_ReturnsTrue()
    {
        // Arrange
        var value = new object();

        // Act
        var result = ValiCraft.Rules.NotNull<object>(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueTypeIsNotNull_ReturnsTrue()
    {
        // Arrange
        int? value = 42;

        // Act
        var result = ValiCraft.Rules.NotNull(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenNullableValueTypeIsNull_ReturnsFalse()
    {
        // Arrange
        int? value = null;

        // Act
        var result = ValiCraft.Rules.NotNull(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmptyStringProvided_ReturnsTrue()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = ValiCraft.Rules.NotNull<string>(value);

        // Assert
        result.Should().BeTrue();
    }
}
