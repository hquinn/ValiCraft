using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class MatchesTests
{
    [Fact]
    public void IsValid_WhenStringMatchesPattern_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Matches("hello123", @"^[a-z]+\d+$");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenStringDoesNotMatchPattern_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Matches("hello", @"^\d+$");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenStringIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Matches(null, @"^\d+$");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithComplexPattern_WorksCorrectly()
    {
        var result = ValiCraft.Rules.Matches("test@example.com", @"^[\w\.-]+@[\w\.-]+\.\w+$");
        result.Should().BeTrue();
    }
}
