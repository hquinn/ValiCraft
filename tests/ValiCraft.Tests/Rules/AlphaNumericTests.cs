using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class AlphaNumericTests
{
    [Fact]
    public void IsValid_WhenOnlyLetters_ReturnsTrue()
    {
        var result = ValiCraft.Rules.AlphaNumeric("HelloWorld");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenOnlyDigits_ReturnsTrue()
    {
        var result = ValiCraft.Rules.AlphaNumeric("123456");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenLettersAndDigits_ReturnsTrue()
    {
        var result = ValiCraft.Rules.AlphaNumeric("Hello123");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenContainsSpace_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric("Hello World");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenContainsSpecialCharacters_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric("Hello@World");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenContainsPunctuation_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric("Hello, World!");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric("");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenWhitespace_ReturnsFalse()
    {
        var result = ValiCraft.Rules.AlphaNumeric("   ");
        result.Should().BeFalse();
    }
}
