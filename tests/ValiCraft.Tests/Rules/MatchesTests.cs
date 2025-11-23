using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class MatchesTests
{
    [Fact]
    public void IsValid_WhenStringMatchesPattern_ReturnsTrue()
    {
        var result = Matches.IsValid("hello123", @"^[a-z]+\d+$");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotMatchPattern_ReturnsFalse()
    {
        var result = Matches.IsValid("hello", @"^\d+$");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = Matches.IsValid(null, @"^\d+$");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithComplexPattern_WorksCorrectly()
    {
        var result = Matches.IsValid("test@example.com", @"^[\w\.-]+@[\w\.-]+\.\w+$");
        result.Should().BeTrue();
    }
}
