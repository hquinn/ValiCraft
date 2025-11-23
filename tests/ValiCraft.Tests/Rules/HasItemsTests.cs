using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class HasItemsTests
{
    [Fact]
    public void IsValid_WhenListHasItems_ReturnsTrue()
    {
        // Arrange
        var value = new List<int> { 1, 2, 3 };

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenListIsEmpty_ReturnsFalse()
    {
        // Arrange
        var value = new List<int>();

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenListIsNull_ReturnsFalse()
    {
        // Arrange
        List<int>? value = null;

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenArrayHasItems_ReturnsTrue()
    {
        // Arrange
        var value = new[] { "a", "b", "c" };

        // Act
        var result = HasItems<string>.IsValid(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenArrayIsEmpty_ReturnsFalse()
    {
        // Arrange
        var value = Array.Empty<string>();

        // Act
        var result = HasItems<string>.IsValid(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEnumerableHasSingleItem_ReturnsTrue()
    {
        // Arrange
        IEnumerable<int> value = new[] { 42 };

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEnumerableIsFromLinqQuery_ReturnsTrue()
    {
        // Arrange
        IEnumerable<int> value = Enumerable.Range(1, 10).Where(x => x > 5);

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEnumerableIsFromLinqQueryWithNoResults_ReturnsFalse()
    {
        // Arrange
        IEnumerable<int> value = Enumerable.Range(1, 10).Where(x => x > 100);

        // Act
        var result = HasItems<int>.IsValid(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenHashSetHasItems_ReturnsTrue()
    {
        // Arrange
        var value = new HashSet<string> { "test" };

        // Act
        var result = HasItems<string>.IsValid(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenHashSetIsEmpty_ReturnsFalse()
    {
        // Arrange
        var value = new HashSet<string>();

        // Act
        var result = HasItems<string>.IsValid(value);

        // Assert
        result.Should().BeFalse();
    }
}
