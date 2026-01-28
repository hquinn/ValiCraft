using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class EqualTests
{
    [Fact]
    public void IsValid_WhenIntegersAreEqual_ReturnsTrue()
    {
        // Arrange
        var value = 42;
        var comparison = 42;

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegersAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var value = 42;
        var comparison = 10;

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringsAreEqual_ReturnsTrue()
    {
        // Arrange
        var value = "test";
        var comparison = "test";

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var value = "test";
        var comparison = "different";

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringsAreCaseSensitive_ReturnsFalse()
    {
        // Arrange
        var value = "Test";
        var comparison = "test";

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenGuidsAreEqual_ReturnsTrue()
    {
        // Arrange
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var value = guid;
        var comparison = guid;

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGuidsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var value = Guid.NewGuid();
        var comparison = Guid.NewGuid();

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDoublesAreEqual_ReturnsTrue()
    {
        // Arrange
        var value = 3.14;
        var comparison = 3.14;

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDoublesAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var value = 3.14;
        var comparison = 2.71;

        // Act
        var result = ValiCraft.Rules.Equal(value, comparison);

        // Assert
        result.Should().BeFalse();
    }
}
