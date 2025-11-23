using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class AlphaNumericTests
{
    [Fact]
    public void IsValid_WhenOnlyLetters_ReturnsTrue()
    {
        var result = AlphaNumeric.IsValid("HelloWorld");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenOnlyDigits_ReturnsTrue()
    {
        var result = AlphaNumeric.IsValid("123456");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLettersAndDigits_ReturnsTrue()
    {
        var result = AlphaNumeric.IsValid("Hello123");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenContainsSpace_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid("Hello World");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenContainsSpecialCharacters_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid("Hello@World");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenContainsPunctuation_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid("Hello, World!");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid("");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenWhitespace_ReturnsFalse()
    {
        var result = AlphaNumeric.IsValid("   ");
        result.Should().BeFalse();
    }
}
