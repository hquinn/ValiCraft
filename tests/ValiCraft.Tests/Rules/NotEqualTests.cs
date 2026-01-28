using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class NotEqualTests
{
    [Fact]
    public void IsValid_WhenIntegersAreNotEqual_ReturnsTrue()
    {
        // Arrange
        var value = 42;
        var comparison = 10;

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenIntegersAreEqual_ReturnsFalse()
    {
        // Arrange
        var value = 42;
        var comparison = 42;

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringsAreNotEqual_ReturnsTrue()
    {
        // Arrange
        var value = "test";
        var comparison = "different";

        // Act
        var result = ValiCraft.Rules.NotEqual<string>(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringsAreEqual_ReturnsFalse()
    {
        // Arrange
        var value = "test";
        var comparison = "test";

        // Act
        var result = ValiCraft.Rules.NotEqual<string>(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringsAreCaseSensitive_ReturnsTrue()
    {
        // Arrange
        var value = "Test";
        var comparison = "test";

        // Act
        var result = ValiCraft.Rules.NotEqual<string>(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGuidsAreNotEqual_ReturnsTrue()
    {
        // Arrange
        var value = Guid.NewGuid();
        var comparison = Guid.NewGuid();

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGuidsAreEqual_ReturnsFalse()
    {
        // Arrange
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var value = guid;
        var comparison = guid;

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDoublesAreNotEqual_ReturnsTrue()
    {
        // Arrange
        var value = 3.14;
        var comparison = 2.71;

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDoublesAreEqual_ReturnsFalse()
    {
        // Arrange
        var value = 3.14;
        var comparison = 3.14;

        // Act
        var result = ValiCraft.Rules.NotEqual(value, comparison);

        // Assert
        result.Should().BeFalse();
    }
}
