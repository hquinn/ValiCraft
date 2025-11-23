using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class MaxAgeTests
{
    [Fact]
    public void IsValid_WhenAgeBelowMaximum_ReturnsTrue()
    {
        var birthDate = DateTime.Today.AddYears(-30);
        var result = MaxAge.IsValid(birthDate, 65);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenAgeExactlyMaximum_ReturnsTrue()
    {
        var birthDate = DateTime.Today.AddYears(-65);
        var result = MaxAge.IsValid(birthDate, 65);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenAgeAboveMaximum_ReturnsFalse()
    {
        var birthDate = DateTime.Today.AddYears(-70);
        var result = MaxAge.IsValid(birthDate, 65);
        result.Should().BeFalse();
    }
}
