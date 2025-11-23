using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class DateBetweenTests
{
    [Fact]
    public void IsValid_WhenDateWithinRange_ReturnsTrue()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var value = new DateTime(2025, 6, 15);
        
        var result = DateBetween.IsValid(value, start, end);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateAtStart_ReturnsTrue()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var value = new DateTime(2025, 1, 1);
        
        var result = DateBetween.IsValid(value, start, end);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateAtEnd_ReturnsTrue()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var value = new DateTime(2025, 12, 31);
        
        var result = DateBetween.IsValid(value, start, end);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenDateBeforeRange_ReturnsFalse()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var value = new DateTime(2024, 12, 31);
        
        var result = DateBetween.IsValid(value, start, end);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDateAfterRange_ReturnsFalse()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var value = new DateTime(2026, 1, 1);
        
        var result = DateBetween.IsValid(value, start, end);
        result.Should().BeFalse();
    }
}
