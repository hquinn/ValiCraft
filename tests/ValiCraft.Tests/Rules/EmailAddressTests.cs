using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class EmailAddressTests
{
    [Fact]
    public void IsValid_WhenValidEmail_ReturnsTrue()
    {
        var result = EmailAddress.IsValid("test@example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenEmailWithSubdomain_ReturnsTrue()
    {
        var result = EmailAddress.IsValid("user@mail.example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenInvalidEmailNoAt_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("testexample.com");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenInvalidEmailNoDomain_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("test@");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = EmailAddress.IsValid(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenWhitespace_ReturnsFalse()
    {
        var result = EmailAddress.IsValid("   ");
        result.Should().BeFalse();
    }
}
