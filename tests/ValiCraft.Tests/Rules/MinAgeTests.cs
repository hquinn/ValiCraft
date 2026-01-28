using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class MinAgeTests
{
    [Fact]
    public void IsValid_WhenAgeMeetsMinimum_ReturnsTrue()
    {
        var birthDate = DateTime.Today.AddYears(-25);
        var result = ValiCraft.Rules.MinAge(birthDate, 18);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenAgeExactlyMinimum_ReturnsTrue()
    {
        var birthDate = DateTime.Today.AddYears(-18);
        var result = ValiCraft.Rules.MinAge(birthDate, 18);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenAgeBelowMinimum_ReturnsFalse()
    {
        var birthDate = DateTime.Today.AddYears(-15);
        var result = ValiCraft.Rules.MinAge(birthDate, 18);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenBirthdayNotYetThisYear_CalculatesCorrectly()
    {
        var birthDate = DateTime.Today.AddYears(-18).AddDays(1);
        var result = ValiCraft.Rules.MinAge(birthDate, 18);
        result.Should().BeFalse();
    }
}
