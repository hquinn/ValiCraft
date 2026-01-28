using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class UrlTests
{
    [Fact]
    public void IsValid_WhenValidHttpUrl_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Url("http://example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValidHttpsUrl_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Url("https://example.com");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenUrlWithPath_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Url("https://example.com/path/to/page");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenUrlWithQueryString_ReturnsTrue()
    {
        var result = ValiCraft.Rules.Url("https://example.com?param=value");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenInvalidUrl_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Url("not a url");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenRelativeUrl_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Url("/path/to/page");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenFtpUrl_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Url("ftp://example.com");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Url(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenEmpty_ReturnsFalse()
    {
        var result = ValiCraft.Rules.Url("");
        result.Should().BeFalse();
    }
}
