using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotDefaultTests
{
    [Fact]
    public void IsValid_WhenIntIsNotDefault_ReturnsTrue()
    {
        // Arrange
        int? value = 42;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntIsDefault_ReturnsFalse()
    {
        // Arrange
        int value = 0;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNullableIntIsNull_ReturnsFalse()
    {
        // Arrange
        int? value = null;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNotDefault_ReturnsTrue()
    {
        // Arrange
        string? value = "test";

        // Act
        var result = ValiCraft.Rules.NotDefault<string>(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        // Arrange
        string? value = null;

        // Act
        var result = ValiCraft.Rules.NotDefault<string>(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenGuidIsNotDefault_ReturnsTrue()
    {
        // Arrange
        Guid? value = Guid.NewGuid();

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGuidIsDefault_ReturnsFalse()
    {
        // Arrange
        Guid value = Guid.Empty;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenBoolIsTrue_ReturnsTrue()
    {
        // Arrange
        bool? value = true;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenBoolIsFalse_ReturnsFalse()
    {
        // Arrange
        bool value = false;

        // Act
        var result = ValiCraft.Rules.NotDefault(value);

        // Assert
        result.Should().BeFalse();
    }
}
