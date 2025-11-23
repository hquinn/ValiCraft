using AwesomeAssertions;
using ValiCraft.Rules;

namespace ValiCraft.Tests.Rules;

public class UrlTests
{
    [Fact]
    public void IsValid_WhenValidHttpUrl_ReturnsTrue()
    {
        var result = Url.IsValid("http://example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValidHttpsUrl_ReturnsTrue()
    {
        var result = Url.IsValid("https://example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenUrlWithPath_ReturnsTrue()
    {
        var result = Url.IsValid("https://example.com/path/to/page");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenUrlWithQueryString_ReturnsTrue()
    {
        var result = Url.IsValid("https://example.com?param=value");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenInvalidUrl_ReturnsFalse()
    {
        var result = Url.IsValid("not a url");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenRelativeUrl_ReturnsFalse()
    {
        var result = Url.IsValid("/path/to/page");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenFtpUrl_ReturnsFalse()
    {
        var result = Url.IsValid("ftp://example.com");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = Url.IsValid(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = Url.IsValid("");
        result.Should().BeFalse();
    }
}
