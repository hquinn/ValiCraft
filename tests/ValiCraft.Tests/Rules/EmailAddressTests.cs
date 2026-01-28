using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class EmailAddressTests
{
    [Fact]
    public void IsValid_WhenValidEmail_ReturnsTrue()
    {
        var result = ValiCraft.Rules.EmailAddress("test@example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEmailWithSubdomain_ReturnsTrue()
    {
        var result = ValiCraft.Rules.EmailAddress("user@mail.example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenInvalidEmailNoAt_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EmailAddress("testexample.com");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenInvalidEmailNoDomain_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EmailAddress("test@");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EmailAddress(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EmailAddress("");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenWhitespace_ReturnsFalse()
    {
        var result = ValiCraft.Rules.EmailAddress("   ");
        result.Should().BeFalse();
    }
}
